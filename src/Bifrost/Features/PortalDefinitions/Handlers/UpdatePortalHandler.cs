using Bifrost.Commands;
using Bifrost.Commands.Portals;
using Bifrost.Data;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Shared;
using MediatR;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class UpdatePortalHandler(IPortalDefinitionRepository repository) : IRequestHandler<UpdatePortalCommand, CommandResponse<UpdatePortalResult>>
{
    private readonly IPortalDefinitionRepository repository = repository;

    public async Task<CommandResponse<UpdatePortalResult>> Handle(UpdatePortalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var definition = await repository.GetByIdAsync(request.Id);
            if (definition is null)
            {
                return CommandResponse<UpdatePortalResult>.Ok(new(request.Id, ErrorDetails: ErrorDetails.SingleError("NotFound", "Portal not found")), "Portal not found");
            }

            definition.Name = request.Name;
            definition.MaxInstanceCount = request.MaxInstanceCount;
            definition.VpnType = request.VpnType;
            definition.VpnConfig = request.VpnConfig ?? string.Empty;

            await repository.UpdateAsync(definition);

            return CommandResponse<UpdatePortalResult>.Ok(new(request.Id), "Portal updated");
        }
        catch (Exception ex) when (RepositoryExceptionHandler.ToErrorDetails(ex) is ErrorDetails error)
        {
            return CommandResponse<UpdatePortalResult>.Problem(new(request.Id, error), "Update failed");
        }
        catch (Exception ex)
        {
            UpdatePortalResult result = new(request.Id ?? "Unknown-Id", ErrorDetails.SingleError(ex.GetType().Name, ex.Message));
            return CommandResponse<UpdatePortalResult>.Problem(result, ex.Message);
        }
    }
}
