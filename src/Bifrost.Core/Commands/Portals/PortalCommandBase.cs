using FluentValidation;

namespace Bifrost.Commands.Portals;

public abstract record PortalCommandBase(
    string Name,
    int MaxInstanceCount,
    string VpnType,
    string? VpnConfig = default);

public class PortalCommandValidatorBase<T> : AbstractValidator<T> where T : PortalCommandBase
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
