using EmployeeManagement.Aggregator.Exceptions;
using EmployeeManagement.DTO.Command;
using EmployeeManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;

namespace EmployeeManagement.Handler.Commands.AssignReportingManager
{
    public class AssignReportingManagerHandler : ICommandHandler<AssignReportingManagerCommand, HandlerResult>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public AssignReportingManagerHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<HandlerResult> HandleAsync(AssignReportingManagerCommand command)
        {
            var employee = await _employeeRepository.GetByIdAsync(command.EmployeeId);
            if (employee == null)
            {
                return HandlerResult.FailureResult(
                    Error.NotFound("EMPLOYEE_NOT_FOUND", $"Employee with ID {command.EmployeeId} not found."));
            }

            var manager = await _employeeRepository.GetByIdAsync(command.ReportingManagerId);
            if (manager == null)
            {
                return HandlerResult.FailureResult(
                    Error.NotFound("MANAGER_NOT_FOUND", $"Reporting Manager with ID {command.ReportingManagerId} not found."));
            }

            try
            {
                employee.AssignReportingManager(command.ReportingManagerId);
                
                await _employeeRepository.UpdateAsync(employee);
                
                return HandlerResult.SuccessResult("Reporting manager assigned successfully.");
            }
            catch (DomainException ex)
            {
                return HandlerResult.FailureResult(
                    Error.Validation("DOMAIN_RULE_VIOLATION", ex.Message));
            }
        }
    }
}
