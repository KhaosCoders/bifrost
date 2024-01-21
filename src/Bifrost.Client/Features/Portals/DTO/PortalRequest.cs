namespace Bifrost.Client.Features.Portals.DTO;

public class PortalRequest
{
    public required string Name { get; set; }

    public int MaxInstanceCount { get; set; }

    public required string VpnType { get; set; }

    public string? VpnConfig { get; set; }
}
