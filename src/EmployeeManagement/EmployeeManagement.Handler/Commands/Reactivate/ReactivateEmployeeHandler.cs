using System;
using System.Threading.Tasks;
using EmployeeManagement.Aggregator.Exceptions;
using EmployeeManagement.DTO.Command;
using EmployeeManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;

namespace EmployeeManagement.Handler.Commands.Reactivate
{
    public class ReactivateEmployeeHandler : ICommandHandler<ReactivateEmployeeCommand, HandlerResult>
    {
        private readonly IEmployeeRepository _repository;

        public ReactivateEmployeeHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<HandlerResult> HandleAsync(ReactivateEmployeeCommand command)
        {
            try
            {
                var employee = await _repository.GetByIdAsync(command.EmployeeId);

                if (employee == null)
                    return HandlerResult.FailureResult("Employee not found.");

                employee.Reactivate();

                await _repository.UpdateAsync(employee);

                return HandlerResult.SuccessResult("Employee reactivated successfully.");
            }
            catch (DomainException ex)
            {
                return HandlerResult.FailureResult(ex.Message);
            }
            catch (Exception ex)
            {
                return HandlerResult.FailureResult($"An error occurred while reactivating the employee: {ex.Message}");
            }
        }
    }
}
