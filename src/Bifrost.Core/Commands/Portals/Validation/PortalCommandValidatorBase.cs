using FluentValidation;

namespace Bifrost.Commands.Portals.Validation;

public class PortalCommandValidatorBase : AbstractValidator<PortalCommandBase>
{
    public PortalCommandValidatorBase()
    {
        RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

        RuleFor(p => p.MaxInstanceCount)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

        RuleFor(p => p.VpnType)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .IsEnumName(typeof(VpnTypes), false);

        RuleFor(p => p.VpnConfig)
            .NotEmpty().WithMessage("{PropertyName} is required.");
    }
}
