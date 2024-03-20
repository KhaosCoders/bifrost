using Bifrost.Utils.Validation;
using Bifrost.Guards;
using Bifrost.Commands.Portals;
using Bifrost.Models.Portals;
using Bifrost.Data;
using Bifrost.Shared;

namespace Bifrost.Features.PortalDefinitions.Services;

internal class PortalDefinitionService : IPortalDefinitionService, IRequestValidator<PortalCommandBase>
{
    private readonly IPortalDefinitionRepository repository;

    public PortalDefinitionService(IPortalDefinitionRepository repository)
    {
        this.Validator = this;
        this.repository = repository;
    }

    internal IRequestValidator<PortalCommandBase> Validator { get; set; }

    ValidationResult IRequestValidator<PortalCommandBase>.ValidateRequest(PortalCommandBase command) =>
        Validation.Validate(
            Validation.Rule(() => command.Name,
                name => Guard.Against.StringIsNullOrWhitespace(name)),
            Validation.Rule(() => command.MaxInstanceCount,
                maxInstanceCount => Guard.Against.IsLessThan(maxInstanceCount, 1)),
            Validation.Rule(() => command.VpnType,
                vpnType => Guard.Against.StringIsNullOrWhitespace(vpnType)),
            Validation.Rule(() => command.VpnConfig,
                vpnConfig => Guard.Against.StringIsNullOrWhitespace(vpnConfig))
        );

    public async Task<CreatePortalDefinitionResult> CreatePortalAsync(CreatePortalCommand request, string creatorName)
    {
        Guard.Against.StringIsNullOrWhitespace(creatorName);

        var validation = Validator.ValidateRequest(request);
        if (!validation.IsValid)
        {
            return new(false, null, validation.GetErrorDetails());
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

        try
        {
            await repository.CreateAsync(definition);
        }
        catch(Exception ex) when (RepositoryExceptionHandler.ToErrorDetails(ex) is ErrorDetails error)
        {
            return new(false, null, error);
        }

        return new(true, definition);
    }

    public async Task<ServiceResult> UpdatePortalAsync(string id, UpdatePortalCommand request)
    {
        Guard.Against.StringIsNullOrWhitespace(id);

        var validation = Validator.ValidateRequest(request);
        if (!validation.IsValid)
        {
            return new(false, validation.GetErrorDetails());
        }

        var definition = await repository.GetByIdAsync(id);
        if (definition is null)
        {
            return new(false, ErrorDetails.SingleError("Portal", "Portal not found"));
        }

        definition.Name = request.Name;
        definition.MaxInstanceCount = request.MaxInstanceCount;
        definition.VpnType = request.VpnType;
        definition.VpnConfig = request.VpnConfig ?? string.Empty;

        await repository.UpdateAsync(definition);

        return new(true);
    }

    public Task<PortalDefinition?> GetPortalAsync(string id)
    {
        Guard.Against.StringIsNullOrWhitespace(id);
        return repository.GetByIdAsync(id);
    }

    public IQueryable<PortalDefinition> GetPortals(int limit, int offset)
    {
        Guard.Against.IsLessThan(limit, 1);
        Guard.Against.IsLessThan(offset, 0);
        return repository.QueryAll().Skip(offset).Take(limit);
    }

    public Task DeletePortalAsync(string id)
    {
        Guard.Against.StringIsNullOrWhitespace(id);
        return repository.DeleteAsync(id);
    }
}
