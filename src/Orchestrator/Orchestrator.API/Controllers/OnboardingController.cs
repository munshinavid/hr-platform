using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using HRPlatform.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.DTO.Onboarding;

namespace Orchestrator.API.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class OnboardingController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public OnboardingController(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpPost]
        public async Task<IActionResult> OnboardEmployee(
            [FromBody] CreateEmployeeOnboardingCommand command)
        {
            var result = await _dispatcher
                .SendCommand<CreateEmployeeOnboardingCommand, HandlerResult<CreateEmployeeOnboardingResponse>>(
                    command);

            if (result.Success)
            {
                return Ok(new
                {
                    message = result.Message,
                    onboarding = result.Data
                });
            }

            return ResultExtensions.MapErrorToActionResult(result.Error);
        }
    }
}

