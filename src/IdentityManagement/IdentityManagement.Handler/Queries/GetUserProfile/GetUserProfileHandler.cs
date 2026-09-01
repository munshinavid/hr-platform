using IdentityManagement.DTO.Query;
using IdentityManagement.DTO.Response;
using IdentityManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;

namespace IdentityManagement.Handler.Queries.GetUserProfile
{
    /// <summary>
    /// Returns the full identity profile for a user.
    /// Used by Orchestrator for composite read aggregation (Employee 360 dashboard).
    ///
    /// Security invariants enforced here:
    ///   - PasswordHash is never mapped to the response.
    ///   - No Employee entity references.
    ///   - UpdatedAt is intentionally excluded from the response (internal audit only).
    /// </summary>
    public class GetUserProfileHandler
        : IQueryHandler<GetUserProfileQuery, HandlerResult<UserProfileResponse>>
    {
        private readonly IIdentityUserRepository _userRepository;

        public GetUserProfileHandler(IIdentityUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<HandlerResult<UserProfileResponse>> HandleAsync(
            GetUserProfileQuery query)
        {
            var user = await _userRepository.GetByIdAsync(query.UserId);

            if (user == null)
            {
                return HandlerResult<UserProfileResponse>.FailureResult(
                    $"User with ID {query.UserId} was not found.");
            }

            return HandlerResult<UserProfileResponse>.SuccessResult(
                new UserProfileResponse
                {
                    UserId    = user.UserId,
                    Email     = user.Email,
                    Role      = user.Role,
                    IsActive  = user.IsActive,
                    CreatedAt = user.CreatedAt
                    // PasswordHash intentionally NOT mapped.
                    // Name intentionally absent — owned by EmployeeManagement.
                    // UpdatedAt intentionally absent — internal audit field.
                },
                "User profile retrieved successfully.");
        }
    }
}
