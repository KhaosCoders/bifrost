using Bifrost.Shared;
using FluentValidation;

namespace Bifrost.Commands.Portals;

public record UpdatePortalCommand(
    string Id,
    string Name,
    int MaxInstanceCount,
    string VpnType,
    string? VpnConfig = default) : PortalCommandBase(Name, MaxInstanceCount, VpnType, VpnConfig), ICommand<UpdatePortalResult>;

public record UpdatePortalResult(string PortalId, ErrorDetails? ErrorDetails = default);

public class UpdatePortalCommandValidator : PortalCommandValidatorBase<UpdatePortalCommand>
{
    public UpdatePortalCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}
