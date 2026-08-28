using Authentication.Aggregator.Mapping;
using Authentication.DTO.Command;

namespace Authentication.Aggregator.Entities
{
    public class UserAggregatorRoot
    {
        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// BCrypt hash of the user's password.
        /// Maps to the existing "Password" column in the User table via AuthDbContext.
        /// Never expose this value in API responses.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public static UserAggregatorRoot MapToAggregator(
            RegisterUserCommand command,
            string passwordHash,
            string role)
        {
            return UserMapper.MapToAggregator(command, passwordHash, role);
        }
    }
}
