using Bifrost.Commands;
using Bifrost.Commands.Portals;
using Bifrost.Data;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Models.Portals;
using Bifrost.Shared;
using MediatR;
using System.Security.Claims;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class CreatePortalHandler(
    IHttpContextAccessor httpContextAccessor,
    IPortalDefinitionRepository repository) : IRequestHandler<CreatePortalCommand, CommandResponse<CreatePortalResult>>
{
    private readonly IPortalDefinitionRepository repository = repository;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public async Task<CommandResponse<CreatePortalResult>> Handle(CreatePortalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (httpContextAccessor?.HttpContext?.User?.Identity is not ClaimsIdentity user
                || string.IsNullOrWhiteSpace(user.Name))
            {
                return CommandResponse<CreatePortalResult>.Problem(new(UnauthorizedRequest: true), "User not logged in");
            }

            PortalDefinition definition = new()
            {
                CreationDate = DateTime.UtcNow,
                CreationUser = user.Name,
                Name = request.Name,
                MaxInstanceCount = request.MaxInstanceCount,
                VpnType = request.VpnType,
                VpnConfig = request.VpnConfig ?? string.Empty
            };

            await repository.CreateAsync(definition);

            return CommandResponse<CreatePortalResult>.Ok(new(definition.Id), "Portal created");
        }
        catch (Exception ex) when (RepositoryExceptionHandler.ToErrorDetails(ex) is ErrorDetails error)
        {
            return CommandResponse<CreatePortalResult>.Problem(new(ErrorDetails: error), "Portal not created");
        }
        catch (Exception ex)
        {
            CreatePortalResult result = new(ErrorDetails: ErrorDetails.SingleError(ex.GetType().Name, ex.Message));
            return CommandResponse<CreatePortalResult>.Problem(result, ex.Message);
        }
    }
}
