using LeaveManagement.DTO.Command;
using LeaveManagement.DTO.Query;
using LeaveManagement.DTO.Response;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
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
            if (!result.Success)
            {
                return BadRequest(new ApiErrorResponse { Message = result.Message ?? "Failed to apply for leave." });
            }
            return Ok(new { message = result.Message, data = result.Data });
        }

        [HttpPost("{leaveRequestId}/approve")]
        public async Task<IActionResult> ApproveLeave([FromRoute] int leaveRequestId, [FromBody] ApproveLeaveCommand command)
        {
            command.LeaveRequestId = leaveRequestId;
            var result = await _dispatcher.SendCommand<ApproveLeaveCommand, HandlerResult>(command);
            if (!result.Success)
            {
                return BadRequest(new ApiErrorResponse { Message = result.Message ?? "Failed to approve leave." });
            }
            return Ok(new { message = result.Message });
        }

        [HttpPost("{leaveRequestId}/reject")]
        public async Task<IActionResult> RejectLeave([FromRoute] int leaveRequestId, [FromBody] RejectLeaveCommand command)
        {
            command.LeaveRequestId = leaveRequestId;
            var result = await _dispatcher.SendCommand<RejectLeaveCommand, HandlerResult>(command);
            if (!result.Success)
            {
                return BadRequest(new ApiErrorResponse { Message = result.Message ?? "Failed to reject leave." });
            }
            return Ok(new { message = result.Message });
        }

        [HttpPost("{leaveRequestId}/cancel")]
        public async Task<IActionResult> CancelLeave([FromRoute] int leaveRequestId)
        {
            var command = new CancelLeaveCommand { LeaveRequestId = leaveRequestId };
            var result = await _dispatcher.SendCommand<CancelLeaveCommand, HandlerResult>(command);
            if (!result.Success)
            {
                return BadRequest(new ApiErrorResponse { Message = result.Message ?? "Failed to cancel leave." });
            }
            return Ok(new { message = result.Message });
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetLeaveBalance([FromQuery] GetLeaveBalanceQuery query)
        {
            var result = await _dispatcher.SendQuery<GetLeaveBalanceQuery, HandlerResult<LeaveBalanceResponse>>(query);
            if (!result.Success)
            {
                return NotFound(new ApiErrorResponse { Message = result.Message ?? "Leave balance not found." });
            }
            return Ok(new { data = result.Data });
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetLeaveRequests([FromQuery] GetLeaveRequestsQuery query)
        {
            var result = await _dispatcher.SendQuery<GetLeaveRequestsQuery, HandlerResult<PagedResponse<LeaveRequestResponse>>>(query);
            if (!result.Success)
            {
                return BadRequest(new ApiErrorResponse { Message = result.Message ?? "Failed to get leave requests." });
            }
            return Ok(new { data = result.Data });
        }

        [HttpGet("requests/{leaveRequestId}")]
        public async Task<IActionResult> GetLeaveRequest([FromRoute] int leaveRequestId)
        {
            var query = new GetLeaveRequestQuery { LeaveRequestId = leaveRequestId };
            var result = await _dispatcher.SendQuery<GetLeaveRequestQuery, HandlerResult<LeaveRequestResponse>>(query);
            if (!result.Success)
            {
                return NotFound(new ApiErrorResponse { Message = result.Message ?? "Leave request not found." });
            }
            return Ok(new { data = result.Data });
        }
    }
}
