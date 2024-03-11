using Bifrost.DTOs;
using Bifrost.Features.PortalDefinitions.Model;

namespace Bifrost.Features.PortalDefinitions.Services;
internal interface IPortalDefinitionService
{
    Task<CreatePortalDefinitionResult> CreatePortalAsync(PortalRequest request, string creatorName);
    Task DeletePortalAsync(string id);
    Task<PortalDefinition?> GetPortalAsync(string id);
    Task<UpdatePortalResult> UpdatePortalAsync(string id, PortalRequest request);
}