using Bifrost.Commands;
using Bifrost.Data;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Models.Portals;
using Bifrost.Queries.Portals;
using Bifrost.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class GetPortalsHandler(IPortalDefinitionRepository repository) : IRequestHandler<GetPortalsQuery, CommandResponse<GetPortalsResult>>
{
    private readonly IPortalDefinitionRepository repository = repository;

    public async Task<CommandResponse<GetPortalsResult>> Handle(GetPortalsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = repository.QueryAll().Skip(request.Offset).Take(request.Limit);
            var portals = query is IAsyncEnumerable<PortalDefinition>
                    ? await query.ToListAsync(cancellationToken: cancellationToken)
                    : [.. query];
            return CommandResponse<GetPortalsResult>.Ok(new(portals), "Got portal list");
        }
        catch (Exception ex) when (RepositoryExceptionHandler.ToErrorDetails(ex) is ErrorDetails error)
        {
            return CommandResponse<GetPortalsResult>.Problem(new([], ErrorDetails: error), "Portal list not accessible");
        }
        catch (Exception ex)
        {
            GetPortalsResult result = new([], ErrorDetails: ErrorDetails.SingleError(ex.GetType().Name, ex.Message));
            return CommandResponse<GetPortalsResult>.Problem(result, ex.Message);
        }
    }
}
