using Bifrost.DTOs;
using Bifrost.Utils.Guards;
using Bifrost.Utils.Validation;
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
                name => Guard.Against.StringIsNullOrWhitespace(name)),
            Validation.Rule(() => request.MaxInstanceCount,
                maxInstanceCount => Guard.Against.IsLessThan(maxInstanceCount, 1)),
            Validation.Rule(() => request.VpnType,
                vpnType => Guard.Against.StringIsNullOrWhitespace(vpnType)),
            Validation.Rule(() => request.VpnConfig,
                vpnConfig => Guard.Against.StringIsNullOrWhitespace(vpnConfig))
        );

    public async Task<CreatePortalDefinitionResult> CreatePortalAsync(PortalRequest request, string creatorName)
    {
        Guard.Against.StringIsNullOrWhitespace(creatorName);

        var validation = Validator.ValidateRequest(request);
        if (!validation.IsValid)
        {
            return new(false, null, validation.Faults);
        }

        PortalDefinition definition = new()
        {
            CreationDate = DateTime.UtcNow,
            CreationUser = creatorName,
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
        Guard.Against.StringIsNullOrWhitespace(id);

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
        Guard.Against.StringIsNullOrWhitespace(id);
        return repository.GetByIdAsync(id);
    }

    public Task DeletePortalAsync(string id)
    {
        Guard.Against.StringIsNullOrWhitespace(id);
        return repository.DeleteAsync(id);
    }
}
