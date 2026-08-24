using EmployeeManagement.Aggregator.Entities;
using EmployeeManagement.DTO.Command;

namespace EmployeeManagement.Aggregator.Mapping
{
    public static class UserMapper
    {
        public static User MapToAggregator(
            CreateEmployeeCommand command,
            string password,
            string role)
        {
            return new User
            {
                Name = command.Name,
                Email = command.Email,
                Password = password,
                Role = role
            };
        }

        public static void MapToAggregator(
            User user,
            UpdateEmployeeCommand command)
        {
            user.Name = command.Name;
            user.Email = command.Email;
        }
    }
}