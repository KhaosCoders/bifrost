using Bifrost.Models.Portals;
using Bifrost.Shared;

namespace Bifrost.Features.PortalDefinitions.Services;

internal record CreatePortalDefinitionResult(bool IsSuccess, PortalDefinition? Portal, ErrorDetails? ErrorDetails = default)
    : ServiceResult(IsSuccess, ErrorDetails);
