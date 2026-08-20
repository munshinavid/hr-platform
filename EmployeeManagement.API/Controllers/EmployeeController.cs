using EmployeeManagement.DTO.Common;
using EmployeeManagement.DTO.Employee;
using EmployeeManagement.Handler.Commands.CreateEmployee;
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

        public EmployeeController(CreateEmployeeHandler createEmployeeHandler)
        {
            _createEmployeeHandler = createEmployeeHandler;
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
    }
}
