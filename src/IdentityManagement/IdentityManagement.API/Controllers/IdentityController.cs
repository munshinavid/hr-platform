using IdentityManagement.DTO.Command;
using IdentityManagement.DTO.Query;
using IdentityManagement.DTO.Response;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using HRPlatform.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public IdentityController(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        // POST: api/users/register
        [HttpPost("users/register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _dispatcher.SendCommand<RegisterUserCommand, HandlerResult>(command);

            return result.ToActionResult();
        }

        // POST: api/auth/login
        [HttpPost("auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _dispatcher.SendCommand<LoginCommand, HandlerResult<IdentityResponse>>(command);

            return result.ToActionResult();
        }

        // POST: api/users/{userId}/deactivate
        [HttpPost("users/{userId}/deactivate")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Deactivate([FromRoute] int userId)
        {
            var command = new DeactivateUserCommand { UserId = userId };
            var result  = await _dispatcher.SendCommand<DeactivateUserCommand, HandlerResult>(command);

            return result.ToActionResult();
        }

        // POST: api/users/{userId}/activate
        [HttpPost("users/{userId}/activate")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Activate([FromRoute] int userId)
        {
            var command = new ActivateUserCommand { UserId = userId };
            var result  = await _dispatcher.SendCommand<ActivateUserCommand, HandlerResult>(command);

            return result.ToActionResult();
        }

        //  Query endpoints 

        // GET: api/users/{userId}/status
        // returns only IsActive.
        [HttpGet("users/{userId}/status")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetUserStatus([FromRoute] int userId)
        {
            var query  = new GetUserStatusQuery { UserId = userId };
            var result = await _dispatcher.SendQuery<GetUserStatusQuery, HandlerResult<UserStatusResponse>>(query);

            if (result.Success)
            {
                return Ok(new { message = result.Message, status = result.Data });
            }

            return ResultExtensions.MapErrorToActionResult(result.Error);
        }

        // GET: api/users/{userId}/profile
        // Full identity profile
        [HttpGet("users/{userId}/profile")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetUserProfile([FromRoute] int userId)
        {
            var query  = new GetUserProfileQuery { UserId = userId };
            var result = await _dispatcher.SendQuery<GetUserProfileQuery, HandlerResult<UserProfileResponse>>(query);

            if (result.Success)
            {
                return Ok(new { message = result.Message, profile = result.Data });
            }

            return ResultExtensions.MapErrorToActionResult(result.Error);
        }
    }
}





