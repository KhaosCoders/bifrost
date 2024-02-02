﻿using Bifrost.Client.Features.Portals.DTO;
using Bifrost.Features.Identity.Model;
using Bifrost.Features.PortalDefinitions.Model;

namespace Bifrost.Features.PortalDefinitions.Services;
internal interface IPortalDefinitionService
{
    Task<CreatePortalDefinitionResult> CreatePortalAsync(PortalRequest request, ApplicationUser creator);
    Task DeletePortalAsync(string id);
    Task<PortalDefinition?> GetPortalAsync(string id);
    Task<UpdatePortalResult> UpdatePortalAsync(string id, PortalRequest request);
}