using EmployeeManagement.Aggregator.Aggregates;
using EmployeeManagement.DTO.Command;

namespace EmployeeManagement.Aggregator.Mapping
{
    public static class EmployeeMapper
    {
        public static EmployeeAggregateRoot MapToAggregator(
            CreateEmployeeCommand command,
            int userId)
        {
            return new EmployeeAggregateRoot
            {
                UserId = userId,
                Name = command.Name,
                Email = command.Email,
                Phone = command.Phone,
                Gender = command.Gender,
                DepartmentId = command.DepartmentId,
                JobTitle = command.JobTitle,
                Salary = command.Salary,
                EmploymentType = command.EmploymentType,
                JoiningDate = command.JoiningDate,
                Status = command.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static void MapToAggregator(
            EmployeeAggregateRoot employee,
            UpdateEmployeeCommand command)
        {
            employee.Name = command.Name;
            employee.Email = command.Email;
            employee.Phone = command.Phone;
            employee.Gender = command.Gender;
            employee.DepartmentId = command.DepartmentId;
            employee.JobTitle = command.JobTitle;
            employee.Salary = command.Salary;
            employee.EmploymentType = command.EmploymentType;
            employee.JoiningDate = command.JoiningDate;
            employee.Status = command.Status;
            employee.UpdatedAt = DateTime.UtcNow;
        }
    }
}
