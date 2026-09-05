using System;
using System.Threading.Tasks;
using EmployeeManagement.DTO.Command;
using EmployeeManagement.DTO.Query;
using EmployeeManagement.DTO.Response;
using HRPlatform.ServiceBus.Abstractions;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using IdentityManagement.DTO.Command;
using LeaveManagement.DTO.Command;
using LeaveManagement.DTO.Response;
using Microsoft.Extensions.Logging;
using Orchestrator.DTO.Offboarding;

namespace Orchestrator.Handler.Offboarding
{
    public class OffboardEmployeeHandler
        : ICommandHandler<OffboardEmployeeCommand, HandlerResult<OffboardEmployeeResponse>>
    {
        private readonly IServiceBus _serviceBus;
        private readonly ILogger<OffboardEmployeeHandler> _logger;

        public OffboardEmployeeHandler(
            IServiceBus serviceBus,
            ILogger<OffboardEmployeeHandler> logger)
        {
            _serviceBus = serviceBus;
            _logger = logger;
        }

        public async Task<HandlerResult<OffboardEmployeeResponse>> HandleAsync(
            OffboardEmployeeCommand command)
        {
            _logger.LogInformation("Offboarding: starting for EmployeeId={EmployeeId}", command.EmployeeId);

            // Step 1: Get Employee
            var empQuery = new GetEmployeeQuery { EmployeeId = command.EmployeeId };
            HandlerResult<EmployeeResponse> empResult;
            try
            {
                empResult = await _serviceBus.SendQueryAsync<GetEmployeeQuery, HandlerResult<EmployeeResponse>>(empQuery);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Offboarding: no handler registered for GetEmployeeQuery.");
                return HandlerResult<OffboardEmployeeResponse>.FailureResult(
                    Error.ServiceUnavailable("SERVICE_UNAVAILABLE", "Employee service is not available."));
            }

            if (!empResult.Success || empResult.Data == null)
            {
                return HandlerResult<OffboardEmployeeResponse>.FailureResult(empResult.Error);
            }

            if (empResult.Data.Status == "Terminated")
            {
                return HandlerResult<OffboardEmployeeResponse>.FailureResult(
                    Error.Conflict("ALREADY_TERMINATED", "Employee is already terminated."));
            }

            int userId = empResult.Data.UserId;

            // Step 2: Terminate Employee
            var terminateCmd = new TerminateEmployeeCommand { EmployeeId = command.EmployeeId };
            HandlerResult terminateResult;
            try
            {
                terminateResult = await _serviceBus.SendCommandAsync<TerminateEmployeeCommand, HandlerResult>(terminateCmd);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Offboarding: no handler registered for TerminateEmployeeCommand.");
                return HandlerResult<OffboardEmployeeResponse>.FailureResult(
                    Error.ServiceUnavailable("SERVICE_UNAVAILABLE", "Employee termination service is not available."));
            }

            if (!terminateResult.Success)
            {
                return HandlerResult<OffboardEmployeeResponse>.FailureResult(terminateResult.Error);
            }

            _logger.LogInformation("Offboarding: Employee {EmployeeId} terminated. Proceeding to deactivate identity {UserId}.", command.EmployeeId, userId);

            // Step 3: Deactivate Identity
            var deactivateCmd = new DeactivateUserCommand { UserId = userId };
            HandlerResult deactivateResult;
            try
            {
                deactivateResult = await _serviceBus.SendCommandAsync<DeactivateUserCommand, HandlerResult>(deactivateCmd);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Offboarding: no handler registered for DeactivateUserCommand.");
                deactivateResult = HandlerResult.FailureResult(
                    Error.ServiceUnavailable("SERVICE_UNAVAILABLE", "Identity service is not available."));
            }

            if (!deactivateResult.Success)
            {
                _logger.LogError("Offboarding: Identity deactivation failed. Triggering compensation for EmployeeId={EmployeeId}. Reason: {Reason}", command.EmployeeId, deactivateResult.Error.Description);
                await CompensateEmployeeTerminationAsync(command.EmployeeId);

                return HandlerResult<OffboardEmployeeResponse>.FailureResult(
                    Error.Failure("DEACTIVATION_FAILED", $"Identity deactivation failed: {deactivateResult.Error.Description}. The employee termination was rolled back/reactivated."));
            }

            _logger.LogInformation("Offboarding: Identity {UserId} deactivated. Proceeding to cancel pending leaves for Employee {EmployeeId}.", userId, command.EmployeeId);

            // Step 4: Cancel pending leaves
            var cancelLeavesCmd = new CancelPendingLeavesCommand { EmployeeId = command.EmployeeId };
            HandlerResult<CancelPendingLeavesResponse> cancelLeavesResult;
            try
            {
                cancelLeavesResult = await _serviceBus.SendCommandAsync<CancelPendingLeavesCommand, HandlerResult<CancelPendingLeavesResponse>>(cancelLeavesCmd);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Offboarding: no handler registered for CancelPendingLeavesCommand.");
                cancelLeavesResult = HandlerResult<CancelPendingLeavesResponse>.FailureResult(
                    Error.ServiceUnavailable("SERVICE_UNAVAILABLE", "Leave management service is not available."));
            }

            var response = new OffboardEmployeeResponse
            {
                EmployeeId = command.EmployeeId,
                UserId = userId,
                Message = "Employee offboarding completed.",
                LeaveCleanupFailed = !cancelLeavesResult.Success,
                LeavesCancelledCount = cancelLeavesResult.Data?.CancelledCount ?? 0
            };

            if (!cancelLeavesResult.Success)
            {
                _logger.LogWarning("Offboarding: Leave cleanup failed for EmployeeId={EmployeeId}. Asymmetric workflow: preserving termination and identity deactivation. Reason: {Reason}", command.EmployeeId, cancelLeavesResult.Error.Description);
                response.Message = $"Employee terminated and Identity deactivated, but leave cleanup failed: {cancelLeavesResult.Error.Description}";
                return HandlerResult<OffboardEmployeeResponse>.SuccessResult(response, response.Message); // Return success with partial failure details
            }

            _logger.LogInformation("Offboarding: completed successfully for EmployeeId={EmployeeId}", command.EmployeeId);
            return HandlerResult<OffboardEmployeeResponse>.SuccessResult(response, response.Message);
        }

        private async Task CompensateEmployeeTerminationAsync(int employeeId)
        {
            var reactivateCmd = new ReactivateEmployeeCommand { EmployeeId = employeeId };
            try
            {
                var compensationResult = await _serviceBus.SendCommandAsync<ReactivateEmployeeCommand, HandlerResult>(reactivateCmd);
                
                if (compensationResult.Success)
                {
                    _logger.LogInformation("Compensation successful: Reactivated employee with EmployeeId={EmployeeId}.", employeeId);
                }
                else
                {
                    _logger.LogError("Compensation failed: Could not reactivate employee with EmployeeId={EmployeeId}. Reason: {Reason}", employeeId, compensationResult.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Compensation failed: An exception occurred while attempting to reactivate employee with EmployeeId={EmployeeId}.", employeeId);
            }
        }
    }
}
