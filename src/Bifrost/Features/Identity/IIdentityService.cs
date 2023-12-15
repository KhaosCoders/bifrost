using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Bifrost.Features.Identity;
public interface IIdentityService
{
    Task<SignInResult> LoginAsync(string username, string password, string? mfaCode, string? mfaRecovery, bool? useCookies, bool? useSessionCookies);
    Task<ClaimsPrincipal?> RefreshAsync(string refreshToken);
    Task<IdentityResult> RegisterAsync(string username, string password, string email);
}