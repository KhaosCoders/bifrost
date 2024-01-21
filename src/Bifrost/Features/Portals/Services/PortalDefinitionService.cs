using Bifrost.Client.Features.Portals.DTO;
using Bifrost.Client.Utils.Guards;
using Bifrost.Client.Utils.Validation;
using Bifrost.Features.Identity.Model;
using Bifrost.Features.Portals.Model;

namespace Bifrost.Features.Portals.Services;

internal class PortalDefinitionService(IPortalDefinitionRepository repository) : IPortalDefinitionService
{
    private readonly IPortalDefinitionRepository repository = repository;

    public async Task<CreatePortalDefinitionResult> CreatePortalAsync(PortalRequest request, ApplicationUser creator)
    {
        var validation = ValidateRequest(request);
        if (!validation.IsValid)
        {
            return new(false, null, validation.Faults);
        }

        PortalDefinition definition = new()
        {
            CreationDate = DateTime.UtcNow,
            CreationUser = creator.UserName!,
            Name = request.Name,
            MaxInstanceCount = request.MaxInstanceCount,
            VpnType = request.VpnType,
            VpnConfig = request.VpnConfig ?? string.Empty
        };

        await repository.CreateAsync(definition);

        return new(true, definition, []);
    }

    private static ValidationResult ValidateRequest(PortalRequest request) =>
        Validator.Validate(
            Validator.Rule(() => request.Name,
                x => Guard.Against.StringIsNullOrWhitespace(x, nameof(request.Name))),
            Validator.Rule(() => request.MaxInstanceCount,
                x => Guard.Against.IsLessThan(x, 0, nameof(request.MaxInstanceCount))),
            Validator.Rule(() => request.VpnType,
                x => Guard.Against.StringIsNullOrWhitespace(x, nameof(request.VpnType)))
        );

    public async Task<UpdatePortalResult> UpdatePortalAsync(string id, PortalRequest request)
    {
        Guard.Against.StringIsNullOrWhitespace(id, nameof(id));

        var validation = ValidateRequest(request);
        if (!validation.IsValid)
        {
            return new(false, validation.Faults);
        }

        var definition = await repository.GetByIdAsync(id);
        if (definition is null)
        {
            return new(false, new[] { new ValidationFault("Portal", "Portal not found") });
        }

        definition.Name = request.Name;
        definition.MaxInstanceCount = request.MaxInstanceCount;
        definition.VpnType = request.VpnType;
        definition.VpnConfig = request.VpnConfig ?? string.Empty;

        await repository.UpdateAsync(definition);

        return new(true, []);
    }

    public Task<PortalDefinition?> GetPortalAsync(string id)
    {
        Guard.Against.StringIsNullOrWhitespace(id, nameof(id));
        return repository.GetByIdAsync(id);
    }

    public Task DeletePortalAsync(string id)
    {
        Guard.Against.StringIsNullOrWhitespace(id, nameof(id));
        return repository.DeleteAsync(id);
    }
}
