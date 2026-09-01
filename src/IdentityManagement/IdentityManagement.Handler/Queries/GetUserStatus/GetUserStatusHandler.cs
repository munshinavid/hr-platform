using IdentityManagement.DTO.Query;
using IdentityManagement.DTO.Response;
using IdentityManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;

namespace IdentityManagement.Handler.Queries.GetUserStatus
{
    /// <summary>
    /// Returns only IsActive for a given UserId.
    /// Designed as a lightweight gatekeeper query for the Orchestrator —
    /// e.g., verifying an account is active before applying leave.
    /// Never returns PasswordHash or any sensitive field.
    /// </summary>
    public class GetUserStatusHandler
        : IQueryHandler<GetUserStatusQuery, HandlerResult<UserStatusResponse>>
    {
        private readonly IIdentityUserRepository _userRepository;

        public GetUserStatusHandler(IIdentityUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<HandlerResult<UserStatusResponse>> HandleAsync(
            GetUserStatusQuery query)
        {
            var user = await _userRepository.GetByIdAsync(query.UserId);

            if (user == null)
            {
                return HandlerResult<UserStatusResponse>.FailureResult(
                    $"User with ID {query.UserId} was not found.");
            }

            return HandlerResult<UserStatusResponse>.SuccessResult(
                new UserStatusResponse
                {
                    UserId   = user.UserId,
                    IsActive = user.IsActive
                },
                "User status retrieved successfully.");
        }
    }
}
