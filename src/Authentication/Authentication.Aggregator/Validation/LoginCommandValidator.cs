using Authentication.DTO.Command;
using FluentValidation;

namespace Authentication.Aggregator.Validation
{
    /// <summary>
    /// Input-only validation for LoginCommand.
    /// User-existence and password-correctness checks are handled in
    /// LoginHandler through IAuthUserRepository, not here.
    /// </summary>
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);
        }
    }
}
