using Bifrost.Models.Portals;

namespace Bifrost.Queries.Portals;

public record GetPortalsQuery(int? Limit, int? Offset) : IQuery<GetPortalsResult>;

public record GetPortalsResult(IList<PortalDefinition> Portals);
