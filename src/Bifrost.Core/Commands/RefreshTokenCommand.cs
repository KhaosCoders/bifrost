using System.Security.Claims;

namespace Bifrost.Commands;

public record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResult>;

public record RefreshTokenResult(ClaimsPrincipal? NewPrincipal = default);
