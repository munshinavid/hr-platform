using IdentityManagement.DTO.Command;
using IdentityManagement.DTO.Response;
using IdentityManagement.Handler.Services;
using IdentityManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;

namespace IdentityManagement.Handler.Commands.Login
{
    public class LoginHandler : ICommandHandler<LoginCommand, HandlerResult<IdentityResponse>>
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly IPasswordHasher     _passwordHasher;
        private readonly IJwtTokenService    _jwtTokenService;

        public LoginHandler(
            IIdentityUserRepository  userRepository,
            IPasswordHasher      passwordHasher,
            IJwtTokenService     jwtTokenService)
        {
            _userRepository  = userRepository;
            _passwordHasher  = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<HandlerResult<IdentityResponse>> HandleAsync(LoginCommand command)
        {
            var user = await _userRepository.GetByEmailAsync(command.Email);

            if (user == null)
            {
                return HandlerResult<IdentityResponse>.FailureResult("Invalid email or password.");
            }

            var isPasswordValid = _passwordHasher.Verify(command.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return HandlerResult<IdentityResponse>.FailureResult("Invalid email or password.");
            }

            // Account lifecycle check — performed AFTER password verification to avoid
            // leaking whether the email exists. A deactivated account returns the same
            // generic error so callers cannot distinguish inactive from wrong-password.
            if (!user.IsActive)
            {
                return HandlerResult<IdentityResponse>.FailureResult("Invalid email or password.");
            }

            var token = _jwtTokenService.GenerateToken(user);

            var identityResponse = new IdentityResponse
            {
                Token            = token,
                TokenType        = "Bearer",
                ExpiresInMinutes = _jwtTokenService.GetExpirationMinutes()
            };

            return HandlerResult<IdentityResponse>.SuccessResult(identityResponse, "Login successful.");
        }
    }
}
