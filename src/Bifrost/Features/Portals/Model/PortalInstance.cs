namespace Bifrost.Features.Portals.Model;

public class PortalInstance
{
    public string Id { get; set; } = default!;

    public required PortalDefinition Portal { get; set; }

    public required string PortalId { get; set; }

    public IList<InstancePortMapping>? Mappings { get; set; }

    public IList<PortalHistory>? History { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public PortalState? CurrentState =>
        History?.LastOrDefault()?.State;

    public DateTime? CreationDate =>
        History?.FirstOrDefault()?.CreationDate;

    public string? CreationUser =>
        History?.FirstOrDefault()?.CreationUser;

    public PortalInstance()
    {
        Id = Guid.NewGuid().ToString();
    }
}
