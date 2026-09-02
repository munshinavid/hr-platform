using System.Threading.Tasks;
using LeaveManagement.DTO.Query;
using LeaveManagement.DTO.Response;
using LeaveManagement.Aggregator.Mapping;
using LeaveManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;

namespace LeaveManagement.Handler.Queries.GetLeaveRequest
{
    public class GetLeaveRequestHandler : IQueryHandler<GetLeaveRequestQuery, HandlerResult<LeaveRequestResponse>>
    {
        private readonly ILeaveRequestRepository _requestRepository;

        public GetLeaveRequestHandler(ILeaveRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<HandlerResult<LeaveRequestResponse>> HandleAsync(GetLeaveRequestQuery query)
        {
            var request = await _requestRepository.GetByIdAsync(query.LeaveRequestId);
            if (request == null)
            {
                return HandlerResult<LeaveRequestResponse>.FailureResult("Leave request not found.");
            }

            var response = LeaveMapper.MapToResponse(request);
            return HandlerResult<LeaveRequestResponse>.SuccessResult(response);
        }
    }
}
