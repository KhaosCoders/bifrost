using Bifrost.Guards;
using Bifrost.Commands.Portals;
using Bifrost.Models.Portals;
using Bifrost.Data;
using Bifrost.Shared;
using Bifrost.Extensions;
using FluentValidation;

namespace Bifrost.Features.PortalDefinitions.Services;

internal class PortalDefinitionService(
    IPortalDefinitionRepository repository,
    IValidator<CreatePortalCommand> createCommandValidator,
    IValidator<UpdatePortalCommand> updateCommandValidator) : IPortalDefinitionService
{
    private readonly IPortalDefinitionRepository repository = repository;

    private readonly IValidator<CreatePortalCommand> createCommandValidator = createCommandValidator;
    private readonly IValidator<UpdatePortalCommand> updateCommandValidator = updateCommandValidator;

    public async Task<CreatePortalDefinitionResult> CreatePortalAsync(CreatePortalCommand request, string creatorName)
    {
        Guard.Against.StringIsNullOrWhitespace(creatorName);

        var validationResult = createCommandValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new(false, null, validationResult.ToErrorDetails());
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

        var validationResult = updateCommandValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new(false, validationResult.ToErrorDetails());
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
