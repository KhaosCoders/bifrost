using Bifrost.Commands;
using Bifrost.Data;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Queries.Portals;
using Bifrost.Shared;
using MediatR;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class GetPortalHandler(IPortalDefinitionRepository repository) : IRequestHandler<GetPortalQuery, CommandResponse<GetPortalResult>>
{
    private readonly IPortalDefinitionRepository repository = repository;

    public async Task<CommandResponse<GetPortalResult>> Handle(GetPortalQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var portal = await repository.GetByIdAsync(request.Id);
            return CommandResponse<GetPortalResult>.Ok(new(portal), "Found Portal");
        }
        catch (Exception ex) when (RepositoryExceptionHandler.ToErrorDetails(ex) is ErrorDetails error)
        {
            return CommandResponse<GetPortalResult>.Problem(new(ErrorDetails: error), "Portal not found");
        }
        catch (Exception ex)
        {
            GetPortalResult result = new(ErrorDetails: ErrorDetails.SingleError(ex.GetType().Name, ex.Message));
            return CommandResponse<GetPortalResult>.Problem(result, ex.Message);
        }
    }
}
