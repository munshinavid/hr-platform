using IdentityManagement.Aggregator.Constants;
using IdentityManagement.Aggregator.Entities;
using IdentityManagement.DTO.Command;
using IdentityManagement.Handler.Services;
using IdentityManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;

namespace IdentityManagement.Handler.Commands.Register
{
    public class RegisterUserHandler : ICommandHandler<RegisterUserCommand, HandlerResult>
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly IPasswordHasher     _passwordHasher;

        public RegisterUserHandler(
            IIdentityUserRepository userRepository,
            IPasswordHasher     passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<HandlerResult> HandleAsync(RegisterUserCommand command)
        {
            var emailExists = await _userRepository.EmailExistsAsync(command.Email);

            if (emailExists)
            {
                return HandlerResult.FailureResult("Email is already registered.");
            }

            var passwordHash = _passwordHasher.Hash(command.Password);

            var user = UserAggregatorRoot.MapToAggregator(
                command,
                passwordHash,
                Roles.Employee);

            var saved = await _userRepository.AddAsync(user);

            if (!saved)
            {
                return HandlerResult.FailureResult("User could not be saved to the database.");
            }

            return HandlerResult.SuccessResult("Registration successful.");
        }
    }
}

