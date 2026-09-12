using IdentityManagement.DTO.Command;
using IdentityManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using Microsoft.Extensions.Logging;

namespace IdentityManagement.Handler.Commands.Deactivate
{
    public class DeactivateUserHandler : ICommandHandler<DeactivateUserCommand, HandlerResult>
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly ILogger<DeactivateUserHandler> _logger;

        public DeactivateUserHandler(
            IIdentityUserRepository userRepository,
            ILogger<DeactivateUserHandler> logger)
        {
            _userRepository = userRepository;
            _logger         = logger;
        }

        public async Task<HandlerResult> HandleAsync(DeactivateUserCommand command)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId);

            if (user == null)
            {
                return HandlerResult.FailureResult(
                    Error.NotFound("USER_NOT_FOUND", $"User with ID {command.UserId} was not found."));
            }

            var changed = user.Deactivate();

            if (!changed)
            {
                _logger.LogWarning(
                    "DeactivateUser: UserId={UserId} is already inactive. No change made.",
                    command.UserId);

                return HandlerResult.FailureResult(
                    Error.Conflict("USER_ALREADY_INACTIVE", $"User {command.UserId} is already inactive."));
            }

            var saved = await _userRepository.UpdateAsync(user);

            if (!saved)
            {
                _logger.LogError(
                    "DeactivateUser: failed to persist deactivation for UserId={UserId}.",
                    command.UserId);

                return HandlerResult.FailureResult(
                    Error.Failure("DEACTIVATE_USER_FAILED", "Account could not be deactivated. Please try again."));
            }

            _logger.LogInformation(
                "DeactivateUser: UserId={UserId} deactivated successfully.",
                command.UserId);

            return HandlerResult.SuccessResult("User account deactivated successfully.");
        }
    }
}
