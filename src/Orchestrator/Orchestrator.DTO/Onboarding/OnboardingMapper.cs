using EmployeeManagement.DTO.Command;
using IdentityManagement.DTO.Command;
using EmployeeManagement.DTO.Response;

namespace Orchestrator.DTO.Onboarding
{
    public static class OnboardingMapper
    {
        public static RegisterUserCommand ToRegisterUserCommand(CreateEmployeeOnboardingCommand command)
        {
            return new RegisterUserCommand
            {
                Email    = command.Email,
                Password = command.Password
            };
        }

        public static CreateEmployeeCommand ToCreateEmployeeCommand(CreateEmployeeOnboardingCommand command, int userId)
        {
            return new CreateEmployeeCommand
            {
                UserId         = userId,
                Name           = command.Name,
                Email          = command.Email,
                Phone          = command.Phone,
                Gender         = command.Gender,
                DepartmentId   = command.DepartmentId,
                JobTitle       = command.JobTitle,
                Salary         = command.Salary,
                EmploymentType = command.EmploymentType,
                JoiningDate    = command.JoiningDate,
                Status         = command.Status
            };
        }

        public static CreateEmployeeOnboardingResponse ToCreateEmployeeOnboardingResponse(int userId, EmployeeResponse employeeResponse)
        {
            return new CreateEmployeeOnboardingResponse
            {
                UserId     = userId,
                EmployeeId = employeeResponse.EmployeeId,
                Name       = employeeResponse.Name,
                Email      = employeeResponse.Email
            };
        }
    }
}
