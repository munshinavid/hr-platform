using EmployeeManagement.Aggregator.Entities;
using EmployeeManagement.DTO.Response;

namespace EmployeeManagement.Aggregator.Mapping
{
    public static class EmployeeResponseMapper
    {
        public static EmployeeResponse MapToResponse(EmployeeAggregatorRoot employee)
        {
            return new EmployeeResponse
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                Email = employee.Email,
                Phone = employee.Phone,
                Gender = employee.Gender,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.DepartmentName,
                JobTitle = employee.JobTitle,
                Salary = employee.Salary,
                EmploymentType = employee.EmploymentType,
                JoiningDate = employee.JoiningDate,
                Status = employee.Status,
                ReportingManagerId = employee.ReportingManagerId,
                TerminationDate = employee.TerminationDate,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt
            };
        }
    }
}
