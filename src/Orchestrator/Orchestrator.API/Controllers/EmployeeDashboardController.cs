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

            if (result.Success)
            {
                return Ok(new
                {
                    message = result.Message,
                    data = result.Data
                });
            }

            return HRPlatform.Shared.Extensions.ResultExtensions.MapErrorToActionResult(result.Error);
        }
    }
}
