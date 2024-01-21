using Bifrost.Data.Base;

namespace Bifrost.Features.PortalDefinitions.Model;

public class PortalDefinition : IEntity
{
    public string Id { get; set; } = default!;

    public required string Name { get; set; }

    public required DateTime CreationDate { get; set; }

    public required string CreationUser { get; set; }

    public int MaxInstanceCount { get; set; }

    public IList<PortalInstance>? Instances { get; set; }

    public IList<PortalPortMapping>? Mappings { get; set; }

    public required string VpnType { get; set; }

    public required string VpnConfig { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public PortalDefinition()
    {
        Id = Guid.NewGuid().ToString();
        MaxInstanceCount = 1;
    }
}
