namespace Bifrost.Models.Portals;

public class PortalHistory
{
    public string Id { get; set; } = default!;

    public required PortalInstance Instance { get; set; }

    public required string InstanceId { get; set; }

    public PortalState State { get; set; }

    public required DateTime CreationDate { get; set; }

    public required string CreationUser { get; set; }

    public PortalHistory()
    {
        Id = Guid.NewGuid().ToString();
    }
}
