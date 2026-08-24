using EmployeeManagement.DTO.Command;
using EmployeeManagement.DTO.Query;
using EmployeeManagement.DTO.Response;
using EmployeeManagement.Handler.Common;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
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
        public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command)
        {
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

        [HttpPut("{employeeId}")]
        public async Task<IActionResult> Update(
            [FromRoute] int employeeId,
            [FromBody] UpdateEmployeeCommand command)
        {
            command.EmployeeId = employeeId;

            var result = await _dispatcher.SendCommand<
                UpdateEmployeeCommand,
                HandlerResult<EmployeeResponse>>(command);

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
            [FromQuery] GetEmployeesQuery query)
        {
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
        public async Task<IActionResult> GetEmployeeById([FromRoute] GetEmployeeQuery query)
        {
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
