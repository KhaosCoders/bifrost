using Bifrost.Commands;
using Bifrost.Data;
using Bifrost.Features.PortalDefinitions.Repositories;
using Bifrost.Models.Portals;
using Bifrost.Queries.Portals;
using Bifrost.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class GetPortalsHandler(IPortalDefinitionRepository repository) : IRequestHandler<GetPortalsQuery, CommandResponse<GetPortalsResult>>
{
    private readonly IPortalDefinitionRepository repository = repository;

    public async Task<CommandResponse<GetPortalsResult>> Handle(GetPortalsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = repository.QueryAll();
            if (!string.IsNullOrWhiteSpace(request.Order))
            {
                if (request.Order.StartsWith('-'))
                {
                    query = query.OrderBy($"{request.Order[1..]} descending");
                }
                else
                {
                    query.OrderBy($"{request.Order} ascending");
                }
            }

            query = query.Skip(request.Offset).Take(request.Limit);

#pragma warning disable AsyncFixer02 // Long-running or blocking operations inside an async method
            var portals = query is IAsyncEnumerable<PortalDefinition>
                    // Production
                    ? query.ToListAsync(cancellationToken: cancellationToken)
                    // Unittests
                    : Task.FromResult(query.ToList());
#pragma warning restore AsyncFixer02 // Long-running or blocking operations inside an async method

            var total = repository.CountAsync();

            await Task.WhenAll(total, portals);
            return CommandResponse<GetPortalsResult>.Ok(new(portals.Result, total.Result), "Got portal list");
        }
        catch (Exception ex) when (RepositoryExceptionHandler.ToErrorDetails(ex) is ErrorDetails error)
        {
            return CommandResponse<GetPortalsResult>.Problem(new([], 0, ErrorDetails: error), "Portal list not accessible");
        }
        catch (Exception ex)
        {
            GetPortalsResult result = new([], 0, ErrorDetails: ErrorDetails.SingleError(ex.GetType().Name, ex.Message));
            return CommandResponse<GetPortalsResult>.Problem(result, ex.Message);
        }
    }
}
