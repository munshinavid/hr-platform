using EmployeeManagement.Aggregator.Constants;
using EmployeeManagement.Aggregator.Entities;
using EmployeeManagement.DTO.Employee;

namespace EmployeeManagement.Handler.Mappers
{
    public static class EmployeeResponseMapper
    {

        public static EmployeeResponse MapToResponse(Employee employee)
        {
            return new EmployeeResponse
            {
                EmployeeId = employee.EmployeeId,

                Name = employee.User?.Name ?? string.Empty,
                Email = employee.User?.Email ?? string.Empty,

                Phone = employee.Phone,
                Gender = employee.Gender,

                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.DepartmentName,

                JobTitle = employee.JobTitle,
                Salary = employee.Salary,
                EmploymentType = employee.EmploymentType,
                JoiningDate = employee.JoiningDate,
                Status = employee.Status
            };
        }
    }
}