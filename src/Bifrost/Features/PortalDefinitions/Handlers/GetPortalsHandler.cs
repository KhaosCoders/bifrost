using Bifrost.Commands;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Queries.Portals;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class GetPortalsHandler(IPortalDefinitionService portalDefinitionService) : IRequestHandler<GetPortalsQuery, CommandResponse<GetPortalsResult>>
{
    public async Task<CommandResponse<GetPortalsResult>> Handle(GetPortalsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = portalDefinitionService.GetPortals(request.Limit ?? 50, request.Offset ?? 0).ToImmutableList();

            return CommandResponse<GetPortalsResult>.Ok(new(result), "Got portal list");
        }
        catch (Exception ex)
        {
            return CommandResponse<GetPortalsResult>.Problem(new ([]), ex.Message);
        }
    }
}
