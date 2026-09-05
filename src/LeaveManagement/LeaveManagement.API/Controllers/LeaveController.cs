using LeaveManagement.DTO.Command;
using LeaveManagement.DTO.Query;
using LeaveManagement.DTO.Response;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using HRPlatform.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LeaveManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public LeaveController(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyLeave([FromBody] ApplyLeaveCommand command)
        {
            var result = await _dispatcher.SendCommand<ApplyLeaveCommand, HandlerResult<LeaveRequestResponse>>(command);
            if (result.Success)
            {
                return Ok(new { message = result.Message, data = result.Data });
            }
            return ResultExtensions.MapErrorToActionResult(result.Error);
        }

        [HttpPost("{leaveRequestId}/approve")]
        public async Task<IActionResult> ApproveLeave([FromRoute] int leaveRequestId, [FromBody] ApproveLeaveCommand command)
        {
            command.LeaveRequestId = leaveRequestId;
            var result = await _dispatcher.SendCommand<ApproveLeaveCommand, HandlerResult>(command);
            return result.ToActionResult();
        }

        [HttpPost("{leaveRequestId}/reject")]
        public async Task<IActionResult> RejectLeave([FromRoute] int leaveRequestId, [FromBody] RejectLeaveCommand command)
        {
            command.LeaveRequestId = leaveRequestId;
            var result = await _dispatcher.SendCommand<RejectLeaveCommand, HandlerResult>(command);
            return result.ToActionResult();
        }

        [HttpPost("{leaveRequestId}/cancel")]
        public async Task<IActionResult> CancelLeave([FromRoute] int leaveRequestId)
        {
            var command = new CancelLeaveCommand { LeaveRequestId = leaveRequestId };
            var result = await _dispatcher.SendCommand<CancelLeaveCommand, HandlerResult>(command);
            return result.ToActionResult();
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetLeaveBalance([FromQuery] GetLeaveBalanceQuery query)
        {
            var result = await _dispatcher.SendQuery<GetLeaveBalanceQuery, HandlerResult<LeaveBalanceResponse>>(query);
            if (result.Success)
            {
                return Ok(new { data = result.Data });
            }
            return ResultExtensions.MapErrorToActionResult(result.Error);
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetLeaveRequests([FromQuery] GetLeaveRequestsQuery query)
        {
            var result = await _dispatcher.SendQuery<GetLeaveRequestsQuery, HandlerResult<PagedResponse<LeaveRequestResponse>>>(query);
            if (result.Success)
            {
                return Ok(new { data = result.Data });
            }
            return ResultExtensions.MapErrorToActionResult(result.Error);
        }

        [HttpGet("requests/{leaveRequestId}")]
        public async Task<IActionResult> GetLeaveRequest([FromRoute] int leaveRequestId)
        {
            var query = new GetLeaveRequestQuery { LeaveRequestId = leaveRequestId };
            var result = await _dispatcher.SendQuery<GetLeaveRequestQuery, HandlerResult<LeaveRequestResponse>>(query);
            if (result.Success)
            {
                return Ok(new { data = result.Data });
            }
            return ResultExtensions.MapErrorToActionResult(result.Error);
        }
    }
}
