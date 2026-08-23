using EmployeeManagement.DTO.Query;
using EmployeeManagement.DTO.Response;
using EmployeeManagement.Aggregator.Mapping;
using EmployeeManagement.Handler.Common;
using EmployeeManagement.Repository.Interfaces;
using EmployeeManagement.Shared.Abstractions;

namespace EmployeeManagement.Handler.Queries.GetEmployees
{
    public class GetEmployeesHandler
        : IQueryHandler<GetEmployeesQuery, HandlerResult<PagedResponse<EmployeeResponse>>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        public GetEmployeesHandler(
            IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public async Task<HandlerResult<PagedResponse<EmployeeResponse>>> HandleAsync(
        GetEmployeesQuery query)
        {
            var (employees, totalCount) =
                await _employeeRepository.GetPagedAsync(
                    query.PageNumber,
                    query.PageSize);

            var items = employees
                .Select(EmployeeResponseMapper.MapToResponse)
                .ToList();

            var pagedResponse = new PagedResponse<EmployeeResponse>
            {
                Items = items,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(
                    totalCount / (double)query.PageSize)
            };

            return HandlerResult<PagedResponse<EmployeeResponse>>.SuccessResult(
                pagedResponse,
                "Employees retrieved successfully.");
        }
    }
}
