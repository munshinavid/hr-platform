using Authentication.Aggregator.Entities;
using Authentication.DTO.Command;

namespace Authentication.Aggregator.Mapping
{
    public static class UserMapper
    {
        public static UserAggregatorRoot MapToAggregator(
            RegisterUserCommand command,
            string passwordHash,
            string role)
        {
            return new UserAggregatorRoot
            {
                Name         = command.Name,
                Email        = command.Email,
                PasswordHash = passwordHash,
                Role         = role
            };
        }
    }
}
