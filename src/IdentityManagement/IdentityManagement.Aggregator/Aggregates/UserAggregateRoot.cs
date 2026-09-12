using IdentityManagement.Aggregator.Exceptions;
using IdentityManagement.Aggregator.Mapping;
using IdentityManagement.DTO.Command;

namespace IdentityManagement.Aggregator.Aggregates
{
    public class UserAggregateRoot
    {
        public int UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public static UserAggregateRoot MapToAggregator(
            RegisterUserCommand command,
            string passwordHash,
            string role)
        {
            return UserMapper.MapToAggregator(command, passwordHash, role);
        }


        public bool Deactivate()
        {
            if (!IsActive)
                return false;

            IsActive  = false;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        public bool Activate()
        {
            if (IsActive)
                return false;   

            IsActive  = true;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
    }
}
