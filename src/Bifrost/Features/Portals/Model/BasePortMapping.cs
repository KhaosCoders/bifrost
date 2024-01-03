namespace Bifrost.Features.Portals.Model;

public abstract class BasePortMapping
{
    public required string MappedPort { get; set; }

    public required string Service { get; set; }
}
