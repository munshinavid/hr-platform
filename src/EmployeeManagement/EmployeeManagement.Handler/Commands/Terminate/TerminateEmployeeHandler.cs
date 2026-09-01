using EmployeeManagement.Aggregator.Exceptions;
using EmployeeManagement.DTO.Command;
using EmployeeManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using System;
using System.Threading.Tasks;

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
                return HandlerResult.FailureResult($"Employee with ID {command.EmployeeId} not found.");
            }

            try
            {
                employee.Terminate();
                
                await _employeeRepository.UpdateAsync(employee);
                
                return HandlerResult.SuccessResult("Employee terminated successfully.");
            }
            catch (DomainException ex)
            {
                return HandlerResult.FailureResult(ex.Message);
            }
        }
    }
}
