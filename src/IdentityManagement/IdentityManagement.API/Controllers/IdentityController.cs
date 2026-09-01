using IdentityManagement.DTO.Command;
using IdentityManagement.DTO.Response;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public IdentityController(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        // POST: api/authentication/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _dispatcher.SendCommand<RegisterUserCommand, HandlerResult>(command);

            if (!result.Success)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = result.Message ?? "Bad request"
                });
            }

            return Ok(new
            {
                message = result.Message
            });
        }

        // POST: api/authentication/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _dispatcher.SendCommand<LoginCommand, HandlerResult<IdentityResponse>>(command);

            if (!result.Success)
            {
                return Unauthorized(new ApiErrorResponse
                {
                    Message = result.Message ?? "Unauthorized"
                });
            }

            return Ok(result.Data);
        }
    }
}




