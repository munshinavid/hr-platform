using EmployeeManagement.Aggregator.Constants;
using EmployeeManagement.DTO.Command;
using FluentValidation;

namespace EmployeeManagement.Aggregator.Validation
{
    public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(2, 100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Phone).NotEmpty().Matches(@"^01[0-9]{9}$")
                .WithMessage("Phone number must be a valid 11-digit number.");
            RuleFor(x => x.Gender).NotEmpty()
                .Must(g => g == Gender.Male || g == Gender.Female || g == Gender.Other)
                .WithMessage("Gender must be Male, Female, or Other.");
            RuleFor(x => x.DepartmentId).GreaterThan(0);
            RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Salary).GreaterThan(0);
            RuleFor(x => x.EmploymentType).NotEmpty()
                .Must(e => e == EmploymentType.FullTime || e == EmploymentType.PartTime || e == EmploymentType.Contract || e == EmploymentType.Intern)
                .WithMessage("Invalid employment type.");
            RuleFor(x => x.JoiningDate).NotEmpty();
            RuleFor(x => x.Status).NotEmpty()
                .Must(s => s == EmployeeStatus.Active || s == EmployeeStatus.Inactive)
                .WithMessage("Status must be Active or Inactive.");
        }
    }
}