using Bifrost.Commands.Portals;
using Bifrost.Models.Portals;

namespace Bifrost.Features.PortalDefinitions.Services;

internal interface IPortalDefinitionService
{
    Task<CreatePortalDefinitionResult> CreatePortalAsync(CreatePortalCommand request, string creatorName);
    Task DeletePortalAsync(string id);
    Task<PortalDefinition?> GetPortalAsync(string id);
    IQueryable<PortalDefinition> GetPortals(int limit, int offset);
    Task<ServiceResult> UpdatePortalAsync(string id, UpdatePortalCommand request);
}