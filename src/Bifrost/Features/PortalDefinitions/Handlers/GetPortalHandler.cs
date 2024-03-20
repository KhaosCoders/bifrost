using Bifrost.Commands;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Queries.Portals;
using MediatR;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class GetPortalHandler(IPortalDefinitionService portalDefinitionService) : IRequestHandler<GetPortalQuery, CommandResponse<GetPortalResult>>
{
    private readonly IPortalDefinitionService portalDefinitionService = portalDefinitionService;

    public async Task<CommandResponse<GetPortalResult>> Handle(GetPortalQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await portalDefinitionService.GetPortalAsync(request.Id);

            if (result != null)
            {
                return CommandResponse<GetPortalResult>.Ok(new(result), "Found Portal");
            }

            return CommandResponse<GetPortalResult>.Problem(new(), "Portal not found");
        }
        catch (Exception ex)
        {
            return CommandResponse<GetPortalResult>.Problem(new(), ex.Message);
        }
    }
}
