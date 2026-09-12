using IdentityManagement.DTO.Command;
using IdentityManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using Microsoft.Extensions.Logging;

namespace IdentityManagement.Handler.Commands.Activate
{
    public class ActivateUserHandler : ICommandHandler<ActivateUserCommand, HandlerResult>
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly ILogger<ActivateUserHandler> _logger;

        public ActivateUserHandler(
            IIdentityUserRepository userRepository,
            ILogger<ActivateUserHandler> logger)
        {
            _userRepository = userRepository;
            _logger         = logger;
        }

        public async Task<HandlerResult> HandleAsync(ActivateUserCommand command)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId);

            if (user == null)
            {
                return HandlerResult.FailureResult(
                    Error.NotFound("USER_NOT_FOUND", $"User with ID {command.UserId} was not found."));
            }

            var changed = user.Activate();

            if (!changed)
            {
                _logger.LogWarning(
                    "ActivateUser: UserId={UserId} is already active. No change made.",
                    command.UserId);

                return HandlerResult.FailureResult(
                    Error.Conflict("USER_ALREADY_ACTIVE", $"User {command.UserId} is already active."));
            }

            var saved = await _userRepository.UpdateAsync(user);

            if (!saved)
            {
                _logger.LogError(
                    "ActivateUser: failed to persist activation for UserId={UserId}.",
                    command.UserId);

                return HandlerResult.FailureResult(
                    Error.Failure("ACTIVATE_USER_FAILED", "Account could not be activated. Please try again."));
            }

            _logger.LogInformation(
                "ActivateUser: UserId={UserId} activated successfully.",
                command.UserId);

            return HandlerResult.SuccessResult("User account activated successfully.");
        }
    }
}
