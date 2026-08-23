using EmployeeManagement.DTO.Common;
using EmployeeManagement.DTO.Employee;
using EmployeeManagement.Handler.Commands.CreateEmployee;
using EmployeeManagement.Handler.Common;
using EmployeeManagement.Handler.Queries.GetEmployee;
using EmployeeManagement.Handler.Queries.GetEmployees;
using EmployeeManagement.Shared.Dispatcher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public EmployeeController(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        // POST: api/Employee
        [HttpPost]
        //[Authorize(Roles = "HR")]
        public async Task<IActionResult> Create(CreateEmployeeRequest request)
        {
            var command = new CreateEmployeeCommand(request);
            var result = await _dispatcher.SendCommand<CreateEmployeeCommand, HandlerResult<EmployeeResponse>>(command);

            if (!result.Success)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = result.Message ?? "Bad request"
                });
            }

            return Ok(new
            {
                message = result.Message,
                employee = result.Data
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees(
            [FromQuery] GetEmployeesRequest request)
        {
            var query = new GetEmployeesQuery(request.PageNumber, request.PageSize);
            var result = await _dispatcher.SendQuery<
                GetEmployeesQuery,
                HandlerResult<PagedResponse<EmployeeResponse>>>(query);
            if (!result.Success)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = result.Message ?? "Bad request"
                });
            }
            return Ok(new
            {
                message = result.Message,
                employees = result.Data
            });
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            var query = new GetEmployeeQuery(employeeId);
            var result = await _dispatcher.SendQuery<GetEmployeeQuery, HandlerResult<EmployeeResponse>>(query);

            if (!result.Success)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = result.Message ?? "Employee not found"
                });
            }

            return Ok(new
            {
                message = result.Message,
                employee = result.Data
            });
        }
    }
}
