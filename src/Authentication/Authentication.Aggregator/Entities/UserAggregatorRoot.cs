using Authentication.Aggregator.Mapping;
using Authentication.DTO.Command;

namespace Authentication.Aggregator.Entities
{
    public class UserAggregatorRoot
    {
        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

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
