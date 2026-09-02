using IdentityManagement.DTO.Command;
using IdentityManagement.DTO.Query;
using IdentityManagement.DTO.Response;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
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

        // ── Authentication endpoints (no auth required — these create/validate tokens) ──

        // POST: api/users/register
        [HttpPost("users/register")]
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

            return Ok(new { message = result.Message });
        }

        // POST: api/auth/login
        [HttpPost("auth/login")]
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

        // ── Account lifecycle endpoints (HR role required) ────────────────────────

        // POST: api/users/{userId}/deactivate
        [HttpPost("users/{userId}/deactivate")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Deactivate([FromRoute] int userId)
        {
            var command = new DeactivateUserCommand { UserId = userId };
            var result  = await _dispatcher.SendCommand<DeactivateUserCommand, HandlerResult>(command);

            if (!result.Success)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = result.Message ?? "Deactivation failed."
                });
            }

            return Ok(new { message = result.Message });
        }

        // POST: api/users/{userId}/activate
        [HttpPost("users/{userId}/activate")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Activate([FromRoute] int userId)
        {
            var command = new ActivateUserCommand { UserId = userId };
            var result  = await _dispatcher.SendCommand<ActivateUserCommand, HandlerResult>(command);

            if (!result.Success)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = result.Message ?? "Activation failed."
                });
            }

            return Ok(new { message = result.Message });
        }

        // ── Query endpoints ────────────────────────────────────────────────────────

        // GET: api/users/{userId}/status
        // Lightweight — returns only IsActive. Used as a quick health-check.
        [HttpGet("users/{userId}/status")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetUserStatus([FromRoute] int userId)
        {
            var query  = new GetUserStatusQuery { UserId = userId };
            var result = await _dispatcher.SendQuery<GetUserStatusQuery, HandlerResult<UserStatusResponse>>(query);

            if (!result.Success)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = result.Message ?? "User not found."
                });
            }

            return Ok(new { message = result.Message, status = result.Data });
        }

        // GET: api/users/{userId}/profile
        // Full identity profile — safe fields only (no PasswordHash).
        [HttpGet("users/{userId}/profile")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetUserProfile([FromRoute] int userId)
        {
            var query  = new GetUserProfileQuery { UserId = userId };
            var result = await _dispatcher.SendQuery<GetUserProfileQuery, HandlerResult<UserProfileResponse>>(query);

            if (!result.Success)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = result.Message ?? "User not found."
                });
            }

            return Ok(new { message = result.Message, profile = result.Data });
        }
    }
}





