using EmployeeManagement.Aggregator.Constants;
using EmployeeManagement.Aggregator.Entities;
using EmployeeManagement.Aggregator.Exceptions;
using EmployeeManagement.Aggregator.Mapping;
using EmployeeManagement.DTO.Command;
using EmployeeManagement.DTO.Response;
using EmployeeManagement.Handler.Common;
using EmployeeManagement.Repository.Interfaces;
using EmployeeManagement.Shared.Abstractions;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Handler.Commands.CreateEmployee
{
    public class CreateEmployeeHandler
        : ICommandHandler<CreateEmployeeCommand, HandlerResult<EmployeeResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly ILogger<CreateEmployeeHandler> _logger;

        public CreateEmployeeHandler(
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IUserRepository userRepository,
            ITransactionManager transactionManager,
            ILogger<CreateEmployeeHandler> logger)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _userRepository = userRepository;
            _transactionManager = transactionManager;
            _logger = logger;
        }

        public async Task<HandlerResult<EmployeeResponse>> HandleAsync(
            CreateEmployeeCommand command)
        {
            try
            {
                await _transactionManager.BeginTransactionAsync();

                string tempPassword = "Default@123";
                string hashedPassword =
                    BCrypt.Net.BCrypt.HashPassword(tempPassword);

                var user = User.MapToAggregator(
                    command,
                    hashedPassword,
                    Roles.Employee
                );

                await _userRepository.AddAsync(user);

                var employee = Employee.MapToAggregator(
                    command,
                    user.UserId
                );

                await _employeeRepository.AddAsync(employee);

                await _transactionManager.CommitAsync();

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
                await _transactionManager.RollbackAsync();

                return HandlerResult<EmployeeResponse>.FailureResult(
                    ex.Message);
            }
            catch (Exception ex)
            {
                await _transactionManager.RollbackAsync();

                _logger.LogError(ex, "Error creating employee.");

                return HandlerResult<EmployeeResponse>.FailureResult(
                    "Employee could not be saved to the database.");
            }
        }
    }
}
