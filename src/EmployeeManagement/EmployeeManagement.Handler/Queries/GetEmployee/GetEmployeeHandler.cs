using EmployeeManagement.DTO.Query;
using EmployeeManagement.DTO.Response;
using EmployeeManagement.Aggregator.Mapping;
using HRPlatform.Shared.Common;
using EmployeeManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;

namespace EmployeeManagement.Handler.Queries.GetEmployee
{
    public class GetEmployeeHandler
        : IQueryHandler<GetEmployeeQuery, HandlerResult<EmployeeResponse>>
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
