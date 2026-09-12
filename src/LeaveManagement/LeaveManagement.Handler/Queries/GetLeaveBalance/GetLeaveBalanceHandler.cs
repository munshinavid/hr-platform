using System.Threading.Tasks;
using LeaveManagement.DTO.Query;
using LeaveManagement.DTO.Response;
using LeaveManagement.Aggregator.Mapping;
using LeaveManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;

namespace LeaveManagement.Handler.Queries.GetLeaveBalance
{
    public class GetLeaveBalanceHandler : IQueryHandler<GetLeaveBalanceQuery, HandlerResult<LeaveBalanceResponse>>
    {
        private readonly ILeaveBalanceRepository _balanceRepository;

        public GetLeaveBalanceHandler(ILeaveBalanceRepository balanceRepository)
        {
            _balanceRepository = balanceRepository;
        }

        public async Task<HandlerResult<LeaveBalanceResponse>> HandleAsync(GetLeaveBalanceQuery query)
        {
            var balance = await _balanceRepository.GetByEmployeeAndTypeAsync(query.EmployeeId, query.LeaveTypeId, query.Year);
            
            if (balance == null)
            {
                return HandlerResult<LeaveBalanceResponse>.FailureResult(
                    Error.NotFound("LEAVE_BALANCE_NOT_FOUND", "Leave balance not found."));
            }

            var response = LeaveMapper.MapToResponse(balance);
            return HandlerResult<LeaveBalanceResponse>.SuccessResult(response);
        }
    }
}
