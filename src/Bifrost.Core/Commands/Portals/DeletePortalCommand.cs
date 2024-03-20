using Bifrost.Shared;

namespace Bifrost.Commands.Portals;

public record DeletePortalCommand(string Id) : ICommand<DeletePortalResult>;

public record DeletePortalResult(bool Deleted, bool NotFound = false, ErrorDetails? ErrorDetails = default);
