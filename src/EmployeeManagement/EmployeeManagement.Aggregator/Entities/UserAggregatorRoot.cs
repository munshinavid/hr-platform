using EmployeeManagement.Aggregator.Mapping;
using EmployeeManagement.DTO.Command;
using EmployeeManagement.DTO.Response;

namespace EmployeeManagement.Aggregator.Entities
{
    public class UserAggregatorRoot
    {
        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public EmployeeAggregatorRoot? Employee { get; set; }

        public static UserAggregatorRoot MapToAggregator(
            CreateEmployeeCommand command,
            string password,
            string role)
        {
            return UserMapper.MapToAggregator(
                command,
                password,
                role);
        }

        public void MapToAggregator(
            UpdateEmployeeCommand command)
        {
            UserMapper.MapToAggregator(
                this,
                command);
        }

        //public UserResponse MapToResponse()
        //{
        //    return UserResponseMapper.MapToResponse(this);
        //}
    }
}