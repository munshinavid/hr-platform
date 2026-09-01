using EmployeeManagement.DTO.Command;
using IdentityManagement.DTO.Command;

namespace Orchestrator.DTO.Onboarding
{
    public static class OnboardingCommandMapper
    {
        public static RegisterUserCommand ToRegisterUserCommand(CreateEmployeeOnboardingCommand command)
        {
            return new RegisterUserCommand
            {
                Name     = command.Name,
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
    }
}
