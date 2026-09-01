using IdentityManagement.DTO.Command;
using IdentityManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using Microsoft.Extensions.Logging;

namespace IdentityManagement.Handler.Commands.Activate
{
    /// <summary>
    /// Re-activates a previously deactivated user account.
    ///
    /// Business rules:
    ///   - Non-existent UserId → failure.
    ///   - Already-active account → failure (idempotent guard).
    ///   - Inactive account → IsActive set to true, UpdatedAt stamped, persisted.
    ///
    /// This is the compensation counterpart of DeactivateUserHandler.
    /// Used by the Offboarding Orchestrator if Step 3 fails and Step 2 must be
    /// rolled back (Phase D).
    /// </summary>
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
                    $"User with ID {command.UserId} was not found.");
            }

            var changed = user.Activate();

            if (!changed)
            {
                _logger.LogWarning(
                    "ActivateUser: UserId={UserId} is already active. No change made.",
                    command.UserId);

                return HandlerResult.FailureResult(
                    $"User {command.UserId} is already active.");
            }

            var saved = await _userRepository.UpdateAsync(user);

            if (!saved)
            {
                _logger.LogError(
                    "ActivateUser: failed to persist activation for UserId={UserId}.",
                    command.UserId);

                return HandlerResult.FailureResult(
                    "Account could not be activated. Please try again.");
            }

            _logger.LogInformation(
                "ActivateUser: UserId={UserId} activated successfully.",
                command.UserId);

            return HandlerResult.SuccessResult("User account activated successfully.");
        }
    }
}
