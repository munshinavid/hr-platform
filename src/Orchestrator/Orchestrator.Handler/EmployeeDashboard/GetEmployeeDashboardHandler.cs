using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagement.DTO.Query;
using EmployeeManagement.DTO.Response;
using HRPlatform.ServiceBus.Abstractions;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using IdentityManagement.DTO.Query;
using IdentityManagement.DTO.Response;
using LeaveManagement.DTO.Query;
using LeaveManagement.DTO.Response;
using Microsoft.Extensions.Logging;
using Orchestrator.DTO.EmployeeDashboard;

namespace Orchestrator.Handler.EmployeeDashboard
{
    public class GetEmployeeDashboardHandler : IQueryHandler<GetEmployeeDashboardQuery, HandlerResult<EmployeeDashboardResponse>>
    {
        private readonly IServiceBus _serviceBus;
        private readonly ILogger<GetEmployeeDashboardHandler> _logger;

        public GetEmployeeDashboardHandler(IServiceBus serviceBus, ILogger<GetEmployeeDashboardHandler> logger)
        {
            _serviceBus = serviceBus;
            _logger = logger;
        }

        public async Task<HandlerResult<EmployeeDashboardResponse>> HandleAsync(GetEmployeeDashboardQuery query)
        {
            _logger.LogInformation("Employee Dashboard: starting aggregation for EmployeeId={EmployeeId}", query.EmployeeId);

            // Step 1: Gatekeeper Validation
            var employeeQuery = new GetEmployeeQuery { EmployeeId = query.EmployeeId };
            HandlerResult<EmployeeResponse> employeeResult;

            try
            {
                employeeResult = await _serviceBus.SendQueryAsync<GetEmployeeQuery, HandlerResult<EmployeeResponse>>(employeeQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Employee Dashboard: Failed to retrieve employee {EmployeeId}", query.EmployeeId);
                return HandlerResult<EmployeeDashboardResponse>.FailureResult("Failed to retrieve employee information.");
            }

            if (!employeeResult.Success || employeeResult.Data == null)
            {
                _logger.LogWarning("Employee Dashboard: Gatekeeper failed for EmployeeId={EmployeeId}. Reason: {Reason}", query.EmployeeId, employeeResult.Message);
                return HandlerResult<EmployeeDashboardResponse>.FailureResult($"Employee not found or could not be retrieved: {employeeResult.Message}");
            }

            // Step 2: Parallel Aggregation for independent downstream queries
            _logger.LogInformation("Employee Dashboard: Gatekeeper succeeded. Starting parallel queries for UserId={UserId}, EmployeeId={EmployeeId}", employeeResult.Data.UserId, query.EmployeeId);

            var identityQuery = new GetUserProfileQuery { UserId = employeeResult.Data.UserId };
            var identityTask = _serviceBus.SendQueryAsync<GetUserProfileQuery, HandlerResult<UserProfileResponse>>(identityQuery);

            int currentYear = DateTime.UtcNow.Year;
            var leaveQuery = new GetAllLeaveBalancesQuery { EmployeeId = query.EmployeeId, Year = currentYear };
            var leaveTask = _serviceBus.SendQueryAsync<GetAllLeaveBalancesQuery, HandlerResult<IEnumerable<LeaveBalanceResponse>>>(leaveQuery);

            try
            {
                await Task.WhenAll(identityTask, leaveTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Employee Dashboard: One or more downstream queries failed.");
                return HandlerResult<EmployeeDashboardResponse>.FailureResult("An error occurred while aggregating downstream services.");
            }

            var identityResult = await identityTask;
            var leaveResult = await leaveTask;

            if (!identityResult.Success || identityResult.Data == null)
            {
                _logger.LogError("Employee Dashboard: Identity query failed for UserId={UserId}. Reason: {Reason}", employeeResult.Data.UserId, identityResult.Message);
                return HandlerResult<EmployeeDashboardResponse>.FailureResult($"Failed to retrieve identity profile: {identityResult.Message}");
            }

            if (!leaveResult.Success || leaveResult.Data == null)
            {
                _logger.LogError("Employee Dashboard: Leave balance query failed for EmployeeId={EmployeeId}. Reason: {Reason}", query.EmployeeId, leaveResult.Message);
                return HandlerResult<EmployeeDashboardResponse>.FailureResult($"Failed to retrieve leave balances: {leaveResult.Message}");
            }

            // Combine the results
            var response = new EmployeeDashboardResponse
            {
                Employee = employeeResult.Data,
                Identity = identityResult.Data,
                LeaveBalances = leaveResult.Data
            };

            _logger.LogInformation("Employee Dashboard: aggregation completed successfully for EmployeeId={EmployeeId}", query.EmployeeId);
            return HandlerResult<EmployeeDashboardResponse>.SuccessResult(response);
        }
    }
}
