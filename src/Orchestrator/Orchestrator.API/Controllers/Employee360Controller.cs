using System.Threading.Tasks;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.DTO.Employee360;
using Microsoft.AspNetCore.Authorization;

namespace Orchestrator.API.Controllers
{
    [Route("api/orchestrator/employees")]
    [ApiController]
    //[Authorize(Policy = "RequireHRRole")] // 
    public class Employee360Controller : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public Employee360Controller(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpGet("{employeeId}/360")]
        public async Task<IActionResult> GetEmployee360(
            [FromRoute] int employeeId)
        {
            var query = new GetEmployee360Query { EmployeeId = employeeId };
            
            var result = await _dispatcher
                .SendQuery<GetEmployee360Query, HandlerResult<Employee360Response>>(query);

            if (!result.Success)
            {
                if (result.Message != null && result.Message.Contains("not found", System.StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new ApiErrorResponse { Message = result.Message });
                }
                return BadRequest(new ApiErrorResponse { Message = result.Message ?? "Failed to retrieve Employee 360 profile." });
            }

            return Ok(new
            {
                message = result.Message,
                data = result.Data
            });
        }
    }
}
