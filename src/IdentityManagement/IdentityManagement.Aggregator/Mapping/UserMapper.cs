using IdentityManagement.Aggregator.Aggregates;
using IdentityManagement.DTO.Command;

namespace IdentityManagement.Aggregator.Mapping
{
    public static class UserMapper
    {
        public static UserAggregateRoot MapToAggregator(
            RegisterUserCommand command,
            string passwordHash,
            string role)
        {
            var now = DateTime.UtcNow;

            return new UserAggregateRoot
            {
                Email        = command.Email,
                PasswordHash = passwordHash,
                Role         = role,
                IsActive     = true,     // all newly registered accounts start active
                CreatedAt    = now,
                UpdatedAt    = now
            };
        }
    }
}


