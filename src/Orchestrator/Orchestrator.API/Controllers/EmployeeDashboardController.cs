using System.Threading.Tasks;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.DTO.EmployeeDashboard;
using Microsoft.AspNetCore.Authorization;

namespace Orchestrator.API.Controllers
{
    [Route("api/orchestrator/employees")]
    [ApiController]
    //[Authorize(Policy = "RequireHRRole")] // 
    public class EmployeeDashboardController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public EmployeeDashboardController(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpGet("{employeeId}/dashboard")]
        public async Task<IActionResult> GetEmployeeDashboard(
            [FromRoute] int employeeId)
        {
            var query = new GetEmployeeDashboardQuery { EmployeeId = employeeId };
            
            var result = await _dispatcher
                .SendQuery<GetEmployeeDashboardQuery, HandlerResult<EmployeeDashboardResponse>>(query);

            if (!result.Success)
            {
                if (result.Message != null && result.Message.Contains("not found", System.StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new ApiErrorResponse { Message = result.Message });
                }
                return BadRequest(new ApiErrorResponse { Message = result.Message ?? "Failed to retrieve Employee Dashboard profile." });
            }

            return Ok(new
            {
                message = result.Message,
                data = result.Data
            });
        }
    }
}
