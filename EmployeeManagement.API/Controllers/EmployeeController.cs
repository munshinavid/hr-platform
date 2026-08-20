using EmployeeManagement.DTO.Common;
using EmployeeManagement.DTO.Employee;
using EmployeeManagement.Handler.Commands.CreateEmployee;
using EmployeeManagement.Handler.Queries.GetEmployee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly CreateEmployeeHandler _createEmployeeHandler;
        private readonly GetEmployeeHandler _getEmployeeHandler;

        public EmployeeController(CreateEmployeeHandler createEmployeeHandler, GetEmployeeHandler getEmployeeHandler)
        {
            _createEmployeeHandler = createEmployeeHandler;
            _getEmployeeHandler = getEmployeeHandler;
        }

        // POST: api/Employee
        [HttpPost]
        //[Authorize(Roles = "HR")]
        public async Task<IActionResult> Create(CreateEmployeeRequest request)
        {
            var command = new CreateEmployeeCommand(request);
            var result = await _createEmployeeHandler.HandleAsync(command);

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

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            var query = new GetEmployeeQuery(employeeId);
            var result = await _getEmployeeHandler.HandleAsync(query);
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
