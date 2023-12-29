namespace Bifrost.Features.Portals.Model;

public class PortalDefinition
{
    public string Id { get; set; } = default!;

    public required string Name { get; set; }

    public required DateTime CreationDate { get; set; }

    public required string CreationUser { get; set; }

    public int MaxInstanceCount { get; set; }

    public IList<PortalInstance>? Instances { get; set; }

    public required string VpnType { get; set; }

    public required string VpnConfig { get; set; }

    public PortalDefinition()
    {
        Instances = new List<PortalInstance>();
        Id = Guid.NewGuid().ToString();
        MaxInstanceCount = 1;
    }
}
