using Authentication.DTO.Command;
using Authentication.DTO.Response;
using Authentication.Handler.Services;
using Authentication.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;

namespace Authentication.Handler.Commands.Login
{
    public class LoginHandler : ICommandHandler<LoginCommand, HandlerResult<AuthResponse>>
    {
        private readonly IAuthUserRepository _userRepository;
        private readonly IPasswordHasher     _passwordHasher;
        private readonly IJwtTokenService    _jwtTokenService;

        public LoginHandler(
            IAuthUserRepository  userRepository,
            IPasswordHasher      passwordHasher,
            IJwtTokenService     jwtTokenService)
        {
            _userRepository  = userRepository;
            _passwordHasher  = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<HandlerResult<AuthResponse>> HandleAsync(LoginCommand command)
        {
            var user = await _userRepository.GetByEmailAsync(command.Email);

            if (user == null)
            {
                return HandlerResult<AuthResponse>.FailureResult("Invalid email or password.");
            }

            var isPasswordValid = _passwordHasher.Verify(command.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return HandlerResult<AuthResponse>.FailureResult("Invalid email or password.");
            }

            var token = _jwtTokenService.GenerateToken(user);

            var authResponse = new AuthResponse
            {
                Token          = token,
                TokenType      = "Bearer",
                ExpiresInMinutes = _jwtTokenService.GetExpirationMinutes()
            };

            return HandlerResult<AuthResponse>.SuccessResult(authResponse, "Login successful.");
        }
    }
}
