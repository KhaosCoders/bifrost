using Bifrost.Models.Portals;

namespace Bifrost.Queries.Portals;

public record GetPortalQuery(string Id) : IQuery<GetPortalResult>;

public record GetPortalResult(PortalDefinition? Portal = default);
