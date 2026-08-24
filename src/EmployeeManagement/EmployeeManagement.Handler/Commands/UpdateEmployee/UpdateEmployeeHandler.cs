using EmployeeManagement.Aggregator.Entities;
using EmployeeManagement.Aggregator.Exceptions;
using EmployeeManagement.DTO.Command;
using EmployeeManagement.DTO.Response;
using EmployeeManagement.Handler.Common;
using EmployeeManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Handler.Commands.UpdateEmployee
{
    public class UpdateEmployeeHandler
        : ICommandHandler<UpdateEmployeeCommand, HandlerResult<EmployeeResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly ILogger<UpdateEmployeeHandler> _logger;

        public UpdateEmployeeHandler(
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository,
            ITransactionManager transactionManager,
            ILogger<UpdateEmployeeHandler> logger)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _transactionManager = transactionManager;
            _logger = logger;
        }

        public async Task<HandlerResult<EmployeeResponse>> HandleAsync(
            UpdateEmployeeCommand command)
        {
            try
            {
                await _transactionManager.BeginTransactionAsync();

                var employee =
                    await _employeeRepository.GetByIdAsync(command.EmployeeId);

                if (employee == null)
                {
                    await _transactionManager.RollbackAsync();

                    return HandlerResult<EmployeeResponse>.FailureResult(
                        "Employee not found.");
                }

                var user =
                    await _userRepository.GetByIdAsync(employee.UserId);

                if (user == null)
                {
                    await _transactionManager.RollbackAsync();

                    return HandlerResult<EmployeeResponse>.FailureResult(
                        "User not found.");
                }

                var userEmailExists =
                    await _userRepository.EmailExistsAsync(
                        command.Email,
                        user.UserId);

                if (userEmailExists)
                {
                    await _transactionManager.RollbackAsync();

                    return HandlerResult<EmployeeResponse>.FailureResult(
                        "Email already exists.");
                }

                var employeeEmailExists =
                    await _employeeRepository.EmailExistsAsync(
                        command.Email,
                        employee.EmployeeId);

                if (employeeEmailExists)
                {
                    await _transactionManager.RollbackAsync();

                    return HandlerResult<EmployeeResponse>.FailureResult(
                        "Email already exists.");
                }

                employee.MapToAggregator(command);

                user.MapToAggregator(command);

                await _userRepository.UpdateAsync(user);

                await _employeeRepository.UpdateAsync(employee);

                await _transactionManager.CommitAsync();

                var updatedEmployee =
                    await _employeeRepository.GetByIdAsync(
                        command.EmployeeId);

                var response =
                    updatedEmployee!.MapToResponse();

                return HandlerResult<EmployeeResponse>.SuccessResult(
                    response,
                    "Employee updated successfully.");
            }
            catch (DomainException ex)
            {
                await _transactionManager.RollbackAsync();

                return HandlerResult<EmployeeResponse>.FailureResult(
                    ex.Message);
            }
            catch (Exception ex)
            {
                await _transactionManager.RollbackAsync();

                _logger.LogError(
                    ex,
                    "Error updating employee.");

                return HandlerResult<EmployeeResponse>.FailureResult(
                    "Employee could not be updated.");
            }
        }
    }
}