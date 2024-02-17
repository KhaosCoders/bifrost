namespace Bifrost.Client.Features.Portals;

public class PortalEventArgs(string portalId) : EventArgs
{
    public string PortalId { get; init; } = portalId;
}
