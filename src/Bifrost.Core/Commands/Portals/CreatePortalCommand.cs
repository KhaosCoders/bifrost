using Bifrost.Shared;

namespace Bifrost.Commands.Portals;

public record CreatePortalCommand(
    string Name,
    int MaxInstanceCount,
    string VpnType,
    string? VpnConfig = default) : PortalCommandBase(Name, MaxInstanceCount, VpnType, VpnConfig), ICommand<CreatePortalResult>;

public record CreatePortalResult(string? PortalId = default, bool UnauthorizedRequest = false, ErrorDetails? ErrorDetails = default);
