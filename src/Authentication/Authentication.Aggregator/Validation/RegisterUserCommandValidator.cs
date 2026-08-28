using Authentication.DTO.Command;
using FluentValidation;

namespace Authentication.Aggregator.Validation
{
    /// <summary>
    /// Input-only validation for RegisterUserCommand.
    /// Database-dependent checks (e.g. email already exists) are handled in
    /// RegisterUserHandler through IAuthUserRepository, not here.
    /// Mirrors the EmployeeManagement.Aggregator.Validation convention.
    /// </summary>
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(2, 100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(20);
        }
    }
}
