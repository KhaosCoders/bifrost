namespace Bifrost.Models.Portals;

public abstract class BasePortMapping
{
    public required string MappedPort { get; set; }

    public required string Service { get; set; }
}
