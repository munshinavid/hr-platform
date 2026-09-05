using EmployeeManagement.Aggregator.Exceptions;
using EmployeeManagement.DTO.Command;
using EmployeeManagement.DTO.Response;
using HRPlatform.Shared.Common;
using EmployeeManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Handler.Commands.UpdateEmployee
{
    public class UpdateEmployeeHandler
        : ICommandHandler<UpdateEmployeeCommand, HandlerResult<EmployeeResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<UpdateEmployeeHandler> _logger;

        public UpdateEmployeeHandler(
            IEmployeeRepository employeeRepository,
            ILogger<UpdateEmployeeHandler> logger)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<HandlerResult<EmployeeResponse>> HandleAsync(
            UpdateEmployeeCommand command)
        {
            try
            {
                var employee =
                    await _employeeRepository.GetByIdAsync(command.EmployeeId);

                if (employee == null)
                {
                    return HandlerResult<EmployeeResponse>.FailureResult(
                        Error.NotFound("EMPLOYEE_NOT_FOUND", $"Employee with ID {command.EmployeeId} not found."));
                }

                // Check Employee.Email uniqueness in the EmployeeManagement context.
                // User.Email uniqueness in the Authentication context is not EM's concern.
                var emailExists =
                    await _employeeRepository.EmailExistsAsync(
                        command.Email,
                        employee.EmployeeId);

                if (emailExists)
                {
                    return HandlerResult<EmployeeResponse>.FailureResult(
                        Error.Conflict("EMPLOYEE_EMAIL_EXISTS", $"An employee with email '{command.Email}' already exists."));
                }

                employee.MapToAggregator(command);

                await _employeeRepository.UpdateAsync(employee);

                var updatedEmployee =
                    await _employeeRepository.GetByIdAsync(command.EmployeeId);

                var response = updatedEmployee!.MapToResponse();

                return HandlerResult<EmployeeResponse>.SuccessResult(
                    response,
                    "Employee updated successfully.");
            }
            catch (DomainException ex)
            {
                return HandlerResult<EmployeeResponse>.FailureResult(
                    Error.Validation("DOMAIN_RULE_VIOLATION", ex.Message));
            }
        }
    }
}