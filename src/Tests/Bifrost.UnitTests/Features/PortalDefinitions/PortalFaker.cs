using Bifrost.Models.Portals;

namespace Bifrost.Tests.Features.PortalDefinitions;

internal static class PortalFaker
{
    public static Faker<PortalDefinition> ValidPortal = new Faker<PortalDefinition>()
        .StrictMode(true)
        .RuleFor(o => o.Id, f => f.Random.Guid().ToString())
        .RuleFor(o => o.Name, f => f.System.FileName())
        .RuleFor(o => o.MaxInstanceCount, f => f.Random.Int(1, 10))
        .RuleFor(o => o.VpnType, f => f.PickRandom<VpnTypes>().ToString())
        .RuleFor(o => o.VpnConfig, f => f.Lorem.Paragraph())
        .RuleFor(o => o.CreationDate, f => f.Date.Past())
        .RuleFor(o => o.CreationUser, f => f.Internet.UserName())
        .RuleFor(o => o.Instances, default(IList<PortalInstance>))
        .RuleFor(o => o.Mappings, default(IList<PortalPortMapping>))
        .RuleFor(o => o.ConcurrencyStamp, default(string));
}
