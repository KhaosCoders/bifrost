using Bifrost.Client.Features.Portals.DTO;
using Bifrost.Client.Utils.Guards;
using Bifrost.Client.Utils.Validation;
using Bifrost.Features.Identity.Model;
using Bifrost.Features.PortalDefinitions.Model;

namespace Bifrost.Features.PortalDefinitions.Services;

internal class PortalDefinitionService : IPortalDefinitionService, IRequestValidator<PortalRequest>
{
    private readonly IPortalDefinitionRepository repository;

    public PortalDefinitionService(IPortalDefinitionRepository repository)
    {
        this.Validator = this;
        this.repository = repository;
    }

    internal IRequestValidator<PortalRequest> Validator { get; set; }

    ValidationResult IRequestValidator<PortalRequest>.ValidateRequest(PortalRequest request) =>
        Validation.Validate(
            Validation.Rule(() => request.Name,
                x => Guard.Against.StringIsNullOrWhitespace(x, nameof(request.Name))),
            Validation.Rule(() => request.MaxInstanceCount,
                x => Guard.Against.IsLessThan(x, 1, nameof(request.MaxInstanceCount))),
            Validation.Rule(() => request.VpnType,
                x => Guard.Against.StringIsNullOrWhitespace(x, nameof(request.VpnType))),
            Validation.Rule(() => request.VpnConfig,
                x => Guard.Against.StringIsNullOrWhitespace(x, nameof(request.VpnType)))
        );

    public async Task<CreatePortalDefinitionResult> CreatePortalAsync(PortalRequest request, ApplicationUser creator)
    {
        var validation = Validator.ValidateRequest(request);
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

    public async Task<UpdatePortalResult> UpdatePortalAsync(string id, PortalRequest request)
    {
        Guard.Against.StringIsNullOrWhitespace(id, nameof(id));

        var validation = Validator.ValidateRequest(request);
        if (!validation.IsValid)
        {
            return new(false, validation.Faults);
        }

        var definition = await repository.GetByIdAsync(id);
        if (definition is null)
        {
            return new(false, [new ValidationFault("Portal", "Portal not found")]);
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
