using EmployeeManagement.Aggregator.Entities;
using EmployeeManagement.Aggregator.Exceptions;
using EmployeeManagement.Aggregator.Mapping;
using EmployeeManagement.DTO.Command;
using EmployeeManagement.DTO.Response;
using EmployeeManagement.Handler.Common;
using EmployeeManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Handler.Commands.CreateEmployee
{
    public class CreateEmployeeHandler
        : ICommandHandler<CreateEmployeeCommand, HandlerResult<EmployeeResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<CreateEmployeeHandler> _logger;

        public CreateEmployeeHandler(
            IEmployeeRepository employeeRepository,
            ILogger<CreateEmployeeHandler> logger)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<HandlerResult<EmployeeResponse>> HandleAsync(
            CreateEmployeeCommand command)
        {
            try
            {
                var employee = EmployeeAggregatorRoot.MapToAggregator(
                    command,
                    command.UserId
                );

                await _employeeRepository.AddAsync(employee);

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
                    ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee.");

                return HandlerResult<EmployeeResponse>.FailureResult(
                    "Employee could not be saved to the database.");
            }
        }
    }
}
