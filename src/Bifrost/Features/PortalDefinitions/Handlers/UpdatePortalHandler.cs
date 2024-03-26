using Bifrost.Commands;
using Bifrost.Commands.Portals;
using Bifrost.Extensions;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Shared;
using FluentValidation;
using MediatR;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class UpdatePortalHandler(
    IPortalDefinitionRepository repository,
    IValidator<UpdatePortalCommand> validator) : IRequestHandler<UpdatePortalCommand, CommandResponse<UpdatePortalResult>>
{
    private readonly IPortalDefinitionRepository repository = repository;
    private readonly IValidator<UpdatePortalCommand> validator = validator;

    public async Task<CommandResponse<UpdatePortalResult>> Handle(UpdatePortalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.Id, nameof(request.Id));

            var result = await UpdatePortalAsync(request);

            if (result.IsSuccess)
            {
                return CommandResponse<UpdatePortalResult>.Ok(new(request.Id), "Portal updated");
            }

            return CommandResponse<UpdatePortalResult>.Problem(new(request.Id, result.ErrorDetails), "Update failed");
        }
        catch (Exception ex)
        {
            UpdatePortalResult result = new(request.Id ?? "Unknown-Id", ErrorDetails.SingleError(ex.GetType().Name, ex.Message));
            return CommandResponse<UpdatePortalResult>.Problem(result, ex.Message);
        }
    }

    public async Task<(bool IsSuccess, ErrorDetails? ErrorDetails)> UpdatePortalAsync(UpdatePortalCommand request)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new(false, validationResult.ToErrorDetails());
        }

        var definition = await repository.GetByIdAsync(request.Id);
        if (definition is null)
        {
            return new(false, ErrorDetails.SingleError("Portal", "Portal not found"));
        }

        definition.Name = request.Name;
        definition.MaxInstanceCount = request.MaxInstanceCount;
        definition.VpnType = request.VpnType;
        definition.VpnConfig = request.VpnConfig ?? string.Empty;

        await repository.UpdateAsync(definition);

        return new(true, default);
    }

}
