using Bifrost.Commands;
using Bifrost.Commands.Portals;
using Bifrost.Data.Base;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Shared;
using MediatR;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class DeletePortalHandler(IPortalDefinitionService portalDefinitionService) : IRequestHandler<DeletePortalCommand, CommandResponse<DeletePortalResult>>
{
    private readonly IPortalDefinitionService portalDefinitionService = portalDefinitionService;

    public async Task<CommandResponse<DeletePortalResult>> Handle(DeletePortalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await portalDefinitionService.DeletePortalAsync(request.Id);

            return CommandResponse<DeletePortalResult>.Ok(new(true), "Portal deleted");
        }
        catch (EntityNotFoundException)
        {
            DeletePortalResult result = new(false, true, ErrorDetails: ErrorDetails.SingleError("NotFound", $"Portal with ID {request.Id} not found"));
            return CommandResponse<DeletePortalResult>.Problem(result, "Portal not found");
        }
        catch (Exception ex)
        {
            DeletePortalResult result = new(false, ErrorDetails: ErrorDetails.SingleError(ex.GetType().Name, ex.Message));
            return CommandResponse<DeletePortalResult>.Problem(result, ex.Message);
        }
    }
}
