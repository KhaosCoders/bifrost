namespace Bifrost.Commands.Portals;

public abstract record PortalCommandBase(
    string Name,
    int MaxInstanceCount,
    string VpnType,
    string? VpnConfig = default);
