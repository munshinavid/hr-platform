using Authentication.Aggregator.Entities;
using Authentication.DTO.Command;

namespace Authentication.Aggregator.Mapping
{
    /// <summary>
    /// Static mapping utility for UserAggregatorRoot.
    /// Mirrors the EmployeeManagement.Aggregator.Mapping static class pattern.
    /// No BCrypt or JWT logic belongs here.
    /// </summary>
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
