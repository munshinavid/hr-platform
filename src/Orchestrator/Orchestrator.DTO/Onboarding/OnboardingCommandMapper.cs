using EmployeeManagement.DTO.Command;
using IdentityManagement.DTO.Command;

namespace Orchestrator.DTO.Onboarding
{
    public static class OnboardingCommandMapper
    {
        /// <summary>
        /// Maps the onboarding command to a RegisterUserCommand.
        /// Name is intentionally excluded — IdentityManagement owns only
        /// the credential (Email + Password). The HR profile name is owned
        /// by EmployeeManagement and is passed via CreateEmployeeCommand below.
        /// </summary>
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
    }
}
