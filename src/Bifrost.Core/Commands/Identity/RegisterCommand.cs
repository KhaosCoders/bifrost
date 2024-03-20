using Bifrost.Shared;

namespace Bifrost.Commands.Identity;

public record RegisterCommand(
    string UserName,
    string Password,
    string Email) : ICommand<RegisterResult>;

public record RegisterResult(bool Success, ErrorDetails? ErrorDetails = default);
