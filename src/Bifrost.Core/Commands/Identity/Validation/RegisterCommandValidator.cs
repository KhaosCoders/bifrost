using FluentValidation;

namespace Bifrost.Commands.Identity.Validation;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required");

        RuleFor(x => x.Username)
            .MinimumLength(3)
            .When(x => !string.IsNullOrWhiteSpace(x.Username));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");

        RuleFor(x => x.Password)
            .MinimumLength(6).WithMessage("Password needs to be at least 6 characters long")
            // Password must contain at least one uppercase letter, one lowercase letter, one number and one special character
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).*$")
                .WithMessage("Password need at least one number, lowercase, uppercase & special character")
            .When(x => !string.IsNullOrWhiteSpace(x.Password));

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email is not valid")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
