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
        private readonly Infrastructure.SafeCommandSender _safeCommandSender;
        private readonly ILogger<OffboardEmployeeHandler> _logger;

        public OffboardEmployeeHandler(
            Infrastructure.SafeCommandSender safeCommandSender,
            ILogger<OffboardEmployeeHandler> logger)
        {
            _safeCommandSender = safeCommandSender;
            _logger = logger;
        }

        public async Task<HandlerResult<OffboardEmployeeResponse>> HandleAsync(
            OffboardEmployeeCommand command)
        {
            // Step 1: Get Employee
            var empQuery = new GetEmployeeQuery { EmployeeId = command.EmployeeId };
            var empResult = await _safeCommandSender.SendQueryAsync<GetEmployeeQuery, HandlerResult<EmployeeResponse>>(empQuery);

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
            var terminateResult = await _safeCommandSender.SendCommandAsync<TerminateEmployeeCommand, HandlerResult>(terminateCmd);

            if (!terminateResult.Success)
            {
                return HandlerResult<OffboardEmployeeResponse>.FailureResult(terminateResult.Error);
            }

            // Step 3: Deactivate Identity
            var deactivateCmd = new DeactivateUserCommand { UserId = userId };
            var deactivateResult = await _safeCommandSender.SendCommandAsync<DeactivateUserCommand, HandlerResult>(deactivateCmd);

            if (!deactivateResult.Success)
            {
                _logger.LogError("Offboarding failed during Identity deactivation. Triggering compensation for EmployeeId={EmployeeId}.", command.EmployeeId);
                await CompensateEmployeeTerminationAsync(command.EmployeeId);

                return HandlerResult<OffboardEmployeeResponse>.FailureResult(
                    Error.Failure("DEACTIVATION_FAILED", $"Identity deactivation failed: {deactivateResult.Error.Description}. The employee termination was rolled back/reactivated."));
            }

            // Step 4: Cancel pending leaves
            var cancelLeavesCmd = new CancelPendingLeavesCommand { EmployeeId = command.EmployeeId };
            var cancelLeavesResult = await _safeCommandSender.SendCommandAsync<CancelPendingLeavesCommand, HandlerResult<CancelPendingLeavesResponse>>(cancelLeavesCmd);

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
                _logger.LogWarning("Offboarding: Leave cleanup failed for EmployeeId={EmployeeId}. Asymmetric workflow: preserving termination and identity deactivation.", command.EmployeeId);
                response.Message = $"Employee terminated and Identity deactivated, but leave cleanup failed: {cancelLeavesResult.Error.Description}";
                return HandlerResult<OffboardEmployeeResponse>.SuccessResult(response, response.Message); // Return success with partial failure details
            }

            return HandlerResult<OffboardEmployeeResponse>.SuccessResult(response, response.Message);
        }

        private async Task CompensateEmployeeTerminationAsync(int employeeId)
        {
            var reactivateCmd = new ReactivateEmployeeCommand { EmployeeId = employeeId };
            var compensationResult = await _safeCommandSender.SendCommandAsync<ReactivateEmployeeCommand, HandlerResult>(reactivateCmd);
            
            if (!compensationResult.Success)
            {
                _logger.LogError("Compensation failed for EmployeeId={EmployeeId}.", employeeId);
            }
        }
    }
}
