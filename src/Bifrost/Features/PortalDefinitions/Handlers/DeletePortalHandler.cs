using Bifrost.Commands;
using Bifrost.Commands.Portals;
using Bifrost.Data;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Shared;
using MediatR;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class DeletePortalHandler(IPortalDefinitionRepository repository) : IRequestHandler<DeletePortalCommand, CommandResponse<DeletePortalResult>>
{
    private readonly IPortalDefinitionRepository repository = repository;

    public async Task<CommandResponse<DeletePortalResult>> Handle(DeletePortalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await repository.DeleteAsync(request.Id);
            return CommandResponse<DeletePortalResult>.Ok(new(true), "Portal deleted");
        }
        catch (Exception ex) when (RepositoryExceptionHandler.ToErrorDetails(ex) is ErrorDetails error)
        {
            return CommandResponse<DeletePortalResult>.Problem(new(false, NotFound: error.ContainsKey("NotFound"), ErrorDetails: error), "Portal not deleted");
        }
        catch (Exception ex)
        {
            DeletePortalResult result = new(false, ErrorDetails: ErrorDetails.SingleError(ex.GetType().Name, ex.Message));
            return CommandResponse<DeletePortalResult>.Problem(result, ex.Message);
        }
    }
}
