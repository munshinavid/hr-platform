using EmployeeManagement.Aggregator.Entities;
using EmployeeManagement.DTO.Command;

namespace EmployeeManagement.Aggregator.Mapping
{
    public static class EmployeeMapper
    {
        public static EmployeeAggregatorRoot MapToAggregator(
            CreateEmployeeCommand command,
            int userId)
        {
            return new EmployeeAggregatorRoot
            {
                Phone = command.Phone,
                Gender = command.Gender,
                DepartmentId = command.DepartmentId,
                JobTitle = command.JobTitle,
                Salary = command.Salary,
                EmploymentType = command.EmploymentType,
                JoiningDate = command.JoiningDate,
                Status = command.Status,
                UserId = userId
            };
        }

        public static void MapToAggregator(
            EmployeeAggregatorRoot employee,
            UpdateEmployeeCommand command)
        {
            employee.Phone = command.Phone;
            employee.Gender = command.Gender;
            employee.DepartmentId = command.DepartmentId;
            employee.JobTitle = command.JobTitle;
            employee.Salary = command.Salary;
            employee.EmploymentType = command.EmploymentType;
            employee.JoiningDate = command.JoiningDate;
            employee.Status = command.Status;
        }
    }
}