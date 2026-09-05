using EmployeeManagement.DTO.Command;
using EmployeeManagement.DTO.Query;
using EmployeeManagement.DTO.Response;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using HRPlatform.Shared.Extensions;
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

        [HttpPost]
        //[Authorize(Roles = "HR")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command)
        {
            var result = await _dispatcher.SendCommand<CreateEmployeeCommand, HandlerResult<EmployeeResponse>>(command);

            if (result.Success)
            {
                return Ok(new
                {
                    message = result.Message,
                    employee = result.Data
                });
            }
            return ResultExtensions.MapErrorToActionResult(result.Error);
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

            if (result.Success)
            {
                return Ok(new
                {
                    message = result.Message,
                    employee = result.Data
                });
            }
            return ResultExtensions.MapErrorToActionResult(result.Error);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees(
            [FromQuery] GetEmployeesQuery query)
        {
            var result = await _dispatcher.SendQuery<
                GetEmployeesQuery,
                HandlerResult<PagedResponse<EmployeeResponse>>>(query);

            if (result.Success)
            {
                return Ok(new
                {
                    message = result.Message,
                    employees = result.Data
                });
            }
            return ResultExtensions.MapErrorToActionResult(result.Error);
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeById([FromRoute] int employeeId)
        {
            var query = new GetEmployeeQuery
            {
                EmployeeId = employeeId
            };
            var result = await _dispatcher.SendQuery<GetEmployeeQuery, HandlerResult<EmployeeResponse>>(query);

            if (result.Success)
            {
                return Ok(new
                {
                    message = result.Message,
                    employee = result.Data
                });
            }
            return ResultExtensions.MapErrorToActionResult(result.Error);
        }

        [HttpPost("{employeeId}/terminate")]
        public async Task<IActionResult> Terminate([FromRoute] int employeeId)
        {
            var command = new TerminateEmployeeCommand { EmployeeId = employeeId };
            var result = await _dispatcher.SendCommand<TerminateEmployeeCommand, HandlerResult>(command);

            return result.ToActionResult();
        }

        [HttpPut("{employeeId}/reporting-manager")]
        public async Task<IActionResult> AssignReportingManager(
            [FromRoute] int employeeId,
            [FromBody] AssignReportingManagerCommand command)
        {
            command.EmployeeId = employeeId;
            var result = await _dispatcher.SendCommand<AssignReportingManagerCommand, HandlerResult>(command);

            return result.ToActionResult();
        }
    }
}
