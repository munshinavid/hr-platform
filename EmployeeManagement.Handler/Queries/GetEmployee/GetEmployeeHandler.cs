using EmployeeManagement.DTO.Employee;
using EmployeeManagement.Handler.Common;
using EmployeeManagement.Handler.Mappers;
using EmployeeManagement.Repository.Interfaces;

namespace EmployeeManagement.Handler.Queries.GetEmployee
{
    public class GetEmployeeHandler
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetEmployeeHandler(
            IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<HandlerResult<EmployeeResponse>> HandleAsync(
            GetEmployeeQuery query)
        {
            var employee = await _employeeRepository
                .GetByIdAsync(query.EmployeeId);

            if (employee == null)
            {
                return HandlerResult<EmployeeResponse>.FailureResult(
                    "Employee not found.");
            }

            var response = EmployeeResponseMapper.MapToResponse(employee);

            return HandlerResult<EmployeeResponse>.SuccessResult(
                response,
                "Employee retrieved successfully.");
        }
    }
}