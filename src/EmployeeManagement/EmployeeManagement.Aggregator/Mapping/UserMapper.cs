using EmployeeManagement.Aggregator.Entities;
using EmployeeManagement.DTO.Command;

namespace EmployeeManagement.Aggregator.Mapping
{
    public static class UserMapper
    {
        public static UserAggregatorRoot MapToAggregator(
            CreateEmployeeCommand command,
            string password,
            string role)
        {
            return new UserAggregatorRoot
            {
                Name = command.Name,
                Email = command.Email,
                Password = password,
                Role = role
            };
        }

        public static void MapToAggregator(
            UserAggregatorRoot user,
            UpdateEmployeeCommand command)
        {
            user.Name = command.Name;
            user.Email = command.Email;
        }
    }
}