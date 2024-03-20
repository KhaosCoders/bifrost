namespace Bifrost.Commands;

public record LoginCommand(
    string Username,
    string Password,
    bool UseCookie,
    bool UseSession,
    string? TwoFactorCode = default,
    string? TwoFactorRecoveryCode = default) : ICommand<LoginResult>;

public record LoginResult(
    bool Succeeded,
    bool RequiresTwoFactor,
    bool IsLockedOut);
