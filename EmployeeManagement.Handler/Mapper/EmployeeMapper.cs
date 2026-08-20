using EmployeeManagement.Aggregator.Constants;
using EmployeeManagement.Aggregator.Entities;
using EmployeeManagement.DTO.Employee;

namespace EmployeeManagement.Handler.Mappers
{
    public static class EmployeeMapper
    {
        public static User MapToUser(
            CreateEmployeeRequest request,
            string hashedPassword)
        {
            return new User
            (
                request.Name,
                request.Email,
                hashedPassword,
                Roles.Employee
            );
        }

        public static Employee MapToEmployee(
            CreateEmployeeRequest request,
            int userId)
        {
            return new Employee
            (
                request.Phone,
                request.Gender,
                request.DepartmentId,
                request.JobTitle,
                request.Salary,
                request.EmploymentType,
                request.JoiningDate,
                request.Status,
                userId
            );
        }

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