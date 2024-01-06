using Bifrost.Client.Features.Identity.DTO;
using System.Net.Http.Json;

namespace Bifrost.Client.Features.Identity;

public class ClientsideLoginAction(HttpClient httpClient) : ILoginAction
{
    public async Task<LoginResult> LoginAsync(string username, string password, bool useCookie, bool useSession)
    {
        string endpoint = $"/identity/login?useCookies={useCookie}&useSessionCookies={useSession}";
        var response = await httpClient.PostAsJsonAsync(endpoint, new LoginRequest() { Username = username, Password = password });
        // TODO: Handle MFA & Lockout
        return new(response?.IsSuccessStatusCode == true, false, false);
    }
}
