using EmployeeManagement.Aggregator.Constants;
using EmployeeManagement.Aggregator.Entities;
using EmployeeManagement.DTO.Employee;
using EmployeeManagement.Handler.Common;
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

            var emailExists = await _employeeRepository.EmailExistsAsync(request.Email);

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

                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    Password = hashedPassword,
                    Role = Roles.Employee
                };

                await _userRepository.AddAsync(user);

                var employee = new Employee
                {
                    Phone = request.Phone,
                    Gender = request.Gender,
                    DepartmentId = request.DepartmentId,
                    JobTitle = request.JobTitle,
                    Salary = request.Salary,
                    EmploymentType = request.EmploymentType,
                    JoiningDate = request.JoiningDate,
                    Status = request.Status,
                    UserId = user.UserId
                };

                await _employeeRepository.AddAsync(employee);

                // Re-fetch with navigation properties
                var createdEmployee = await _employeeRepository.GetByIdAsync(employee.EmployeeId);

                var response = MapToResponse(createdEmployee!);

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

        private static EmployeeResponse MapToResponse(Employee employee)
        {
            return new EmployeeResponse
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.User?.Name ?? string.Empty,
                Email = employee.User?.Email ?? string.Empty,
                Phone = employee.Phone,
                Gender = employee.Gender,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.DepartmentName,
                JobTitle = employee.JobTitle,
                Salary = employee.Salary,
                EmploymentType = employee.EmploymentType,
                JoiningDate = employee.JoiningDate,
                Status = employee.Status
            };
        }
    }
}
