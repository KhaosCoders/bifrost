﻿namespace Bifrost.Client.Features.Identity.Actions;

public interface ILoginAction
{
    public Task<LoginResult> LoginAsync(string username, string password, bool useCookie, bool useSession);
}

public record LoginResult(bool Succeeded, bool RequiresTwoFactor, bool IsLockedOut);