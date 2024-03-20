using Bifrost.Commands;
using Bifrost.Commands.Portals;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Shared;
using MediatR;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class UpdatePortalHandler(IPortalDefinitionService portalDefinitionService) : IRequestHandler<UpdatePortalCommand, CommandResponse<UpdatePortalResult>>
{
    private readonly IPortalDefinitionService portalDefinitionService = portalDefinitionService;

    public async Task<CommandResponse<UpdatePortalResult>> Handle(UpdatePortalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.Id, nameof(request.Id));

            ServiceResult result = await portalDefinitionService.UpdatePortalAsync(request.Id, request);

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
}
