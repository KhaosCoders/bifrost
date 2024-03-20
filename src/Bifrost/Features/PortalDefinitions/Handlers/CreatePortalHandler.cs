using Bifrost.Commands;
using Bifrost.Commands.Portals;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Shared;
using MediatR;
using System.Security.Claims;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class CreatePortalHandler(IHttpContextAccessor httpContextAccessor, IPortalDefinitionService portalDefinitionService) : IRequestHandler<CreatePortalCommand, CommandResponse<CreatePortalResult>>
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IPortalDefinitionService portalDefinitionService = portalDefinitionService;

    public async Task<CommandResponse<CreatePortalResult>> Handle(CreatePortalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (httpContextAccessor?.HttpContext?.User?.Identity is not ClaimsIdentity user
                || string.IsNullOrWhiteSpace(user.Name))
            {
                return CommandResponse<CreatePortalResult>.Problem(new(UnauthorizedRequest: true), "User not logged in");
            }

            var result = await portalDefinitionService.CreatePortalAsync(request, user.Name);

            if (result.IsSuccess && result.Portal != null)
            {
                return CommandResponse<CreatePortalResult>.Ok(new(result.Portal.Id), "Portal created");
            }

            return CommandResponse<CreatePortalResult>.Problem(new(ErrorDetails: result.ErrorDetails), "Portal not created");
        }
        catch (Exception ex)
        {
            CreatePortalResult result = new(ErrorDetails: ErrorDetails.SingleError(ex.GetType().Name, ex.Message));
            return CommandResponse<CreatePortalResult>.Problem(result, ex.Message);
        }
    }
}
