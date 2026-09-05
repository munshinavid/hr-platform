using System.Threading.Tasks;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.DTO.Offboarding;

namespace Orchestrator.API.Controllers
{
    [Route("api/orchestrator/employees")]
    [ApiController]
    public class OffboardingController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public OffboardingController(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpPost("{employeeId}/offboard")]
        public async Task<IActionResult> OffboardEmployee(
            [FromRoute] int employeeId)
        {
            var command = new OffboardEmployeeCommand { EmployeeId = employeeId };
            
            var result = await _dispatcher
                .SendCommand<OffboardEmployeeCommand, HandlerResult<OffboardEmployeeResponse>>(command);

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
