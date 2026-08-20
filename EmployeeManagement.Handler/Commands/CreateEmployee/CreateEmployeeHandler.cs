using EmployeeManagement.Aggregator.Constants;
using EmployeeManagement.Aggregator.Entities;
using EmployeeManagement.DTO.Employee;
using EmployeeManagement.Handler.Common;
using EmployeeManagement.Handler.Mappers;
using EmployeeManagement.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Handler.Commands.CreateEmployee
{
    public class CreateEmployeeHandler
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateEmployeeHandler> _logger;

        public CreateEmployeeHandler(
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IUserRepository userRepository,
            ILogger<CreateEmployeeHandler> logger)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<HandlerResult<EmployeeResponse>> HandleAsync(CreateEmployeeCommand command)
        {
            var request = command.Request;

            // Database-dependent business checks through Repository

            var emailExists = await _userRepository.EmailExistsAsync(request.Email);

            if (emailExists)
            {
                return HandlerResult<EmployeeResponse>.FailureResult(
                    "An employee with this email already exists.");
            }

            var department = await _departmentRepository.GetByIdAsync(request.DepartmentId);

            if (department == null)
            {
                return HandlerResult<EmployeeResponse>.FailureResult(
                    "Department not found.");
            }

            // Create domain entities

            try
            {
                string tempPassword = "Default@123";
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(tempPassword);

                //var user = EmployeeMapper.MapToUser(request, hashedPassword);
                var user= User.Create
                (
                    request.Name,
                    request.Email,
                    hashedPassword,
                    Roles.Employee
                );

                await _userRepository.AddAsync(user);

                //var employee = EmployeeMapper.MapToEmployee(request, user.UserId);
                var employee = Employee.Create
                (
                    request.Phone,
                    request.Gender,
                    request.DepartmentId,
                    request.JobTitle,
                    request.Salary,
                    request.EmploymentType,
                    request.JoiningDate,
                    request.Status,
                    user.UserId
                );

                await _employeeRepository.AddAsync(employee);

                // Re-fetch with navigation properties
                var createdEmployee = await _employeeRepository.GetByIdAsync(employee.EmployeeId);

                var response = EmployeeResponseMapper.MapToResponse(createdEmployee!);

                return HandlerResult<EmployeeResponse>.SuccessResult(
                    response,
                    "Employee created successfully.");
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
