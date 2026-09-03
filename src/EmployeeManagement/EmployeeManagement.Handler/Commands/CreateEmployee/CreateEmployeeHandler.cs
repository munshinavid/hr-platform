using EmployeeManagement.Aggregator.Entities;
using EmployeeManagement.Aggregator.Exceptions;
using EmployeeManagement.Aggregator.Mapping;
using EmployeeManagement.DTO.Command;
using EmployeeManagement.DTO.Response;
using HRPlatform.Shared.Common;
using EmployeeManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Handler.Commands.CreateEmployee
{
    public class CreateEmployeeHandler
        : ICommandHandler<CreateEmployeeCommand, HandlerResult<EmployeeResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<CreateEmployeeHandler> _logger;

        public CreateEmployeeHandler(
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            ILogger<CreateEmployeeHandler> logger)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _logger = logger;
        }

        public async Task<HandlerResult<EmployeeResponse>> HandleAsync(
            CreateEmployeeCommand command)
        {
            var department = await _departmentRepository.GetByIdAsync(command.DepartmentId);
            if (department == null)
            {
                return HandlerResult<EmployeeResponse>.FailureResult(
                    Error.NotFound("DEPARTMENT_NOT_FOUND", $"Department with ID {command.DepartmentId} does not exist."));
            }

            var emailExists = await _employeeRepository.EmailExistsAsync(command.Email);
            if (emailExists)
            {
                return HandlerResult<EmployeeResponse>.FailureResult(
                    Error.Conflict("EMPLOYEE_EMAIL_EXISTS", $"An employee with email '{command.Email}' already exists."));
            }

            try
            {
                var employee = EmployeeAggregatorRoot.MapToAggregator(
                    command,
                    command.UserId
                );

                var saved = await _employeeRepository.AddAsync(employee);
                if (!saved)
                {
                    return HandlerResult<EmployeeResponse>.FailureResult(
                        Error.Failure("EMPLOYEE_SAVE_FAILED", "Failed to save employee record to database."));
                }

                var createdEmployee =
                    await _employeeRepository.GetByIdAsync(employee.EmployeeId);

                var response =
                    createdEmployee!.MapToResponse();

                return HandlerResult<EmployeeResponse>.SuccessResult(
                    response,
                    "Employee created successfully.");
            }
            catch (DomainException ex)
            {
                return HandlerResult<EmployeeResponse>.FailureResult(
                    Error.Validation("DOMAIN_RULE_VIOLATION", ex.Message));
            }
        }
    }
}
