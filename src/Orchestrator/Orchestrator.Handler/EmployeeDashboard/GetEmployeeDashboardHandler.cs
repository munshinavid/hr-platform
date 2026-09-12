using EmployeeManagement.DTO.Query;
using EmployeeManagement.DTO.Response;
using HRPlatform.ServiceBus.Abstractions;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using IdentityManagement.DTO.Query;
using IdentityManagement.DTO.Response;
using LeaveManagement.DTO.Query;
using LeaveManagement.DTO.Response;
using Orchestrator.DTO.EmployeeDashboard;
using Orchestrator.Handler.Common;

namespace Orchestrator.Handler.EmployeeDashboard
{
    public class GetEmployeeDashboardHandler : IQueryHandler<GetEmployeeDashboardQuery, HandlerResult<EmployeeDashboardResponse>>
    {
        private readonly SafeCommandSender _safeCommandSender;

        public GetEmployeeDashboardHandler(
            SafeCommandSender safeCommandSender)
        {
            _safeCommandSender = safeCommandSender;
        }

        public async Task<HandlerResult<EmployeeDashboardResponse>> HandleAsync(GetEmployeeDashboardQuery query)
        {
            // Step 1: Gatekeeper Validation
            var employeeQuery = new GetEmployeeQuery { EmployeeId = query.EmployeeId };
            var employeeResult = await _safeCommandSender.SendQueryAsync<GetEmployeeQuery, HandlerResult<EmployeeResponse>>(employeeQuery);

            if (!employeeResult.Success || employeeResult.Data == null)
            {
                return HandlerResult<EmployeeDashboardResponse>.FailureResult(employeeResult.Error);
            }

            // Step 2: Parallel Aggregation 
            var identityQuery = new GetUserProfileQuery { UserId = employeeResult.Data.UserId };
            var identityTask = _safeCommandSender.SendQueryAsync<GetUserProfileQuery, HandlerResult<UserProfileResponse>>(identityQuery);

            int currentYear = DateTime.UtcNow.Year;
            var leaveQuery = new GetAllLeaveBalancesQuery { EmployeeId = query.EmployeeId, Year = currentYear };
            var leaveTask = _safeCommandSender.SendQueryAsync<GetAllLeaveBalancesQuery, HandlerResult<IEnumerable<LeaveBalanceResponse>>>(leaveQuery);

            await Task.WhenAll(identityTask, leaveTask);

            var identityResult = await identityTask;
            var leaveResult = await leaveTask;

            if (!identityResult.Success || identityResult.Data == null)
            {
                return HandlerResult<EmployeeDashboardResponse>.FailureResult(identityResult.Error);
            }

            if (!leaveResult.Success || leaveResult.Data == null)
            {
                return HandlerResult<EmployeeDashboardResponse>.FailureResult(leaveResult.Error);
            }

            // Combining the results
            var response = new EmployeeDashboardResponse
            {
                Employee = employeeResult.Data,
                Identity = identityResult.Data,
                LeaveBalances = leaveResult.Data
            };

            return HandlerResult<EmployeeDashboardResponse>.SuccessResult(response);
        }
    }
}
