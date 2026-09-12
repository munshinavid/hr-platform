using IdentityManagement.DTO.Command;
using IdentityManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;

namespace IdentityManagement.Handler.Commands.Delete
{
    public class DeleteUserHandler : ICommandHandler<DeleteUserCommand, HandlerResult>
    {
        private readonly IIdentityUserRepository _userRepository;

        public DeleteUserHandler(IIdentityUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<HandlerResult> HandleAsync(DeleteUserCommand command)
        {
            var result = await _userRepository.DeleteAsync(command.UserId);
            if (!result)
            {
                return HandlerResult.FailureResult(
                    Error.Failure("USER_DELETE_FAILED", "User not found or could not be deleted."));
            }

            return HandlerResult.SuccessResult("User deleted successfully.");
        }
    }
}

