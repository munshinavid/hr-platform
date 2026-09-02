using System.Linq;
using System.Threading.Tasks;
using LeaveManagement.DTO.Query;
using LeaveManagement.DTO.Response;
using LeaveManagement.Aggregator.Mapping;
using LeaveManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;

namespace LeaveManagement.Handler.Queries.GetLeaveRequests
{
    public class GetLeaveRequestsHandler : IQueryHandler<GetLeaveRequestsQuery, HandlerResult<PagedResponse<LeaveRequestResponse>>>
    {
        private readonly ILeaveRequestRepository _requestRepository;

        public GetLeaveRequestsHandler(ILeaveRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<HandlerResult<PagedResponse<LeaveRequestResponse>>> HandleAsync(GetLeaveRequestsQuery query)
        {
            var (requests, totalCount) = await _requestRepository.GetPagedAsync(
                query.EmployeeId,
                query.LeaveTypeId,
                query.Status,
                query.PageNumber,
                query.PageSize);

            var responseList = requests.Select(LeaveMapper.MapToResponse).ToList();
            var pagedResponse = new PagedResponse<LeaveRequestResponse>(responseList, totalCount, query.PageNumber, query.PageSize);

            return HandlerResult<PagedResponse<LeaveRequestResponse>>.SuccessResult(pagedResponse);
        }
    }
}
