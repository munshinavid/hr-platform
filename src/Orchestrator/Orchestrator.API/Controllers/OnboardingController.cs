using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.DTO.Onboarding;
using Orchestrator.Handler.Onboarding;

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
            [FromBody] CreateEmployeeOnboardingRequest request)
        {
            var command = new CreateEmployeeOnboardingCommand
            {
                Request = request
            };

            var result = await _dispatcher
                .SendCommand<CreateEmployeeOnboardingCommand, HandlerResult<CreateEmployeeOnboardingResponse>>(
                    command);

            if (!result.Success)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = result.Message ?? "Onboarding failed."
                });
            }

            return Ok(new
            {
                message  = result.Data!.Message,
                onboarding = result.Data
            });
        }
    }
}

