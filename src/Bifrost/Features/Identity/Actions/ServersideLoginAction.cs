using Bifrost.Client.Features.Identity.Actions;
using Bifrost.Features.Identity.Services;

namespace Bifrost.Features.Identity.Actions;

public class ServersideLoginAction(IIdentityService identityService) : ILoginAction
{
    public async Task<LoginResult> LoginAsync(string username, string password, bool useCookie, bool useSession)
    {
        var result = await identityService.LoginAsync(username, password, null, null, useCookie, useSession);
        return new LoginResult(result.Succeeded, result.RequiresTwoFactor, result.IsLockedOut);
    }
}
