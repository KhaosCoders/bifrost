namespace Bifrost.Features.Portals.Model;

public class PortalInstance
{
    public string Id { get; set; } = default!;

    public required PortalDefinition Portal { get; set; }

    public PortalHistory? History { get; set; }

    public IList<PortMapping> Mappings { get; set; }

    public PortalInstance()
    {
        Mappings = new List<PortMapping>();
        Id = Guid.NewGuid().ToString();
    }
}
