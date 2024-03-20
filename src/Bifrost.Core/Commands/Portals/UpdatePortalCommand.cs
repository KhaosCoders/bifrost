using Bifrost.Shared;

namespace Bifrost.Commands.Portals;

public record UpdatePortalCommand(
    string Name,
    int MaxInstanceCount,
    string VpnType,
    string? VpnConfig = default,
    string? Id = default) : PortalCommandBase(Name, MaxInstanceCount, VpnType, VpnConfig), ICommand<UpdatePortalResult>;

public record UpdatePortalResult(string PortalId, ErrorDetails? ErrorDetails = default);
