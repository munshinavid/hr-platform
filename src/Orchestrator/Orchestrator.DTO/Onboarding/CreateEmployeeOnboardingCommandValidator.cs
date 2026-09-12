using FluentValidation;

namespace Orchestrator.DTO.Onboarding
{
    public class CreateEmployeeOnboardingCommandValidator : AbstractValidator<CreateEmployeeOnboardingCommand>
    {
        public CreateEmployeeOnboardingCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(20).WithMessage("Password cannot exceed 20 characters.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .Matches(@"^01[0-9]{9}$").WithMessage("Phone number must be a valid 11-digit Bangladeshi number starting with 01.");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .Must(g => g == "Male" || g == "Female" || g == "Other")
                .WithMessage("Gender must be Male, Female, or Other.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("A valid DepartmentId is required.");

            RuleFor(x => x.JobTitle)
                .NotEmpty().WithMessage("JobTitle is required.")
                .MaximumLength(100).WithMessage("JobTitle cannot exceed 100 characters.");

            RuleFor(x => x.Salary)
                .GreaterThan(0).WithMessage("Salary must be greater than zero.");

            RuleFor(x => x.EmploymentType)
                .NotEmpty().WithMessage("EmploymentType is required.")
                .Must(e => e == "Full-Time" || e == "Part-Time" || e == "Contract" || e == "Intern")
                .WithMessage("EmploymentType must be Full-Time, Part-Time, Contract, or Intern.");

            RuleFor(x => x.JoiningDate)
                .NotEmpty().WithMessage("JoiningDate is required.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => s == "Active" || s == "Inactive")
                .WithMessage("Status must be Active or Inactive.");
        }
    }
}

