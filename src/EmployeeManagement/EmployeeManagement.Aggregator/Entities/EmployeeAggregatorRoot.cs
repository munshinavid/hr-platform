using EmployeeManagement.Aggregator.Exceptions;
using EmployeeManagement.Aggregator.Mapping;
using EmployeeManagement.DTO.Command;
using EmployeeManagement.DTO.Response;

namespace EmployeeManagement.Aggregator.Entities
{
    public class EmployeeAggregatorRoot
    {
        public int EmployeeId { get; set; }

        public string Phone { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public string EmploymentType { get; set; } = string.Empty;

        public DateTime JoiningDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public int UserId { get; set; }

        public DepartmentAggregatorRoot? Department { get; set; }

        public UserAggregatorRoot User { get; set; } = null!;

        public static void ValidateBusinessRules(
            decimal salary,
            DateTime joiningDate)
        {
            if (salary < 0)
                throw new DomainException(
                    "Employee salary cannot be negative.");

            if (joiningDate > DateTime.UtcNow)
                throw new DomainException(
                    "Joining date cannot be in the future.");
        }

        public static EmployeeAggregatorRoot MapToAggregator(
            CreateEmployeeCommand command,
            int userId)
        {
            ValidateBusinessRules(
                command.Salary,
                command.JoiningDate);

            return EmployeeMapper.MapToAggregator(
                command,
                userId);
        }

        public void MapToAggregator(
            UpdateEmployeeCommand command)
        {
            ValidateBusinessRules(
                command.Salary,
                command.JoiningDate);

            EmployeeMapper.MapToAggregator(
                this,
                command);
        }

        public EmployeeResponse MapToResponse()
        {
            return EmployeeResponseMapper.MapToResponse(this);
        }
    }
}