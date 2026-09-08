using EmployeeManagement.Aggregator.Exceptions;
using EmployeeManagement.DTO.Command;
using EmployeeManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;

namespace EmployeeManagement.Handler.Commands.Terminate
{
    public class TerminateEmployeeHandler : ICommandHandler<TerminateEmployeeCommand, HandlerResult>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public TerminateEmployeeHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<HandlerResult> HandleAsync(TerminateEmployeeCommand command)
        {
            var employee = await _employeeRepository.GetByIdAsync(command.EmployeeId);

            if (employee == null)
            {
                return HandlerResult.FailureResult(
                    Error.NotFound("EMPLOYEE_NOT_FOUND", $"Employee with ID {command.EmployeeId} not found."));
            }

            try
            {
                employee.Terminate();
            }
            catch (DomainException ex)
            {
                return HandlerResult.FailureResult(
                    Error.Validation("DOMAIN_RULE_VIOLATION", ex.Message));
            }

            await _employeeRepository.UpdateAsync(employee);
            
            return HandlerResult.SuccessResult("Employee terminated successfully.");
        }
    }
}
