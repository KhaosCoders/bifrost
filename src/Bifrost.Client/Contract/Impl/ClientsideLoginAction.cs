
using System.Net.Http.Json;

namespace Bifrost.Client.Contract.Impl;

public class ClientsideLoginAction(HttpClient httpClient) : ILoginAction
{
    public async Task<LoginResult> LoginAsync(string username, string password, bool useCookie, bool useSession)
    {
        string endpoint = $"/identity/login?useCookies={useCookie}&useSessionCookies={useSession}";
        var response = await httpClient.PostAsJsonAsync(endpoint, new { username, password });
        // TODO: Handle MFA & Lockout
        return new(response?.IsSuccessStatusCode == true, false, false);
    }
}
