using Bifrost.Features.Identity.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Bifrost.Features.Identity.Services;

/// <summary>
/// Provides methods for registration, logging in and logging out using ASP.NET Core Identity.
/// </summary>
/// <param name="signInManager"></param>
public class IdentityService(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IUserStore<ApplicationUser> userStore,
    IOptionsMonitor<BearerTokenOptions> bearerTokenOptions,
    TimeProvider timeProvider,
    IHttpContextAccessor httpContextAccessor,
    ILogger<IdentityService> logger) : IIdentityService
{
    // Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.
    private static readonly EmailAddressAttribute emailAddressAttribute = new();

    /// <summary>
    /// Logs the user in using ASP.NET Core Identity.
    /// </summary>
    /// <param name="username">The user's username</param>
    /// <param name="password">The user's password</param>
    /// <param name="mfaCode">optional Two-Factor authentication code</param>
    /// <param name="mfaRecovery">optional Two-Factor recovery code</param>
    /// <param name="useCookies">set cookies or bearer-token</param>
    /// <param name="useSessionCookies">set session-cookie</param>
    /// <returns>result of the sign-in</returns>
    public async Task<SignInResult> LoginAsync(string username, string password, string? mfaCode, string? mfaRecovery, bool? useCookies, bool? useSessionCookies)
    {
        var useCookieScheme = useCookies == true || useSessionCookies == true;
        var isPersistent = useCookies == true && useSessionCookies != true;
        signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

        // Clear the existing external cookie to ensure a clean login process
        if (httpContextAccessor.HttpContext is HttpContext httpContext && httpContext.User.Identity?.IsAuthenticated == true)
        {
            await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }

        var result = await signInManager.PasswordSignInAsync(username, password, isPersistent, lockoutOnFailure: true);

        if (!result.RequiresTwoFactor)
        {
            logger.LogInformation("User {Name} logged in using password", username);
            return result;
        }

        if (!string.IsNullOrEmpty(mfaCode))
        {
            result = await signInManager.TwoFactorAuthenticatorSignInAsync(mfaCode, isPersistent, rememberClient: isPersistent);

            if (result.Succeeded)
            {
                logger.LogInformation("User {Name} logged in using mfa code", username);
                return result;
            }
        }

        if (!string.IsNullOrEmpty(mfaRecovery))
        {
            result = await signInManager.TwoFactorRecoveryCodeSignInAsync(mfaRecovery);

            if (result.Succeeded)
            {
                logger.LogInformation("User {Name} logged in using recovery code", username);
                return result;
            }
        }

        logger.LogWarning("Login failed for user {Name}", username);
        return result;
    }

    /// <summary>
    /// Registers a new user using ASP.NET Core Identity.
    /// </summary>
    ///<param name="username">The user's username</param>
    /// <param name="password">The user's password</param>
    /// <param name="email">The user's email</param>
    /// <returns>result of registration</returns>
    /// <exception cref="NotSupportedException"></exception>
    public async Task<IdentityResult> RegisterAsync(string username, string password, string email)
    {
        if (!userManager.SupportsUserEmail) throw new NotSupportedException($"{nameof(IdentityService)} requires a user store with email support.");

        if (string.IsNullOrEmpty(email) || !emailAddressAttribute.IsValid(email))
        {
            return IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email));
        }

        ApplicationUser user = new();
        await userStore.SetUserNameAsync(user, username, CancellationToken.None);
        await ((IUserEmailStore<ApplicationUser>)userStore).SetEmailAsync(user, email, CancellationToken.None);
        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            logger.LogInformation("User {Name} registered", username);
        }
        else
        {
            logger.LogWarning("Registration failed for user {name}", username);
        }

        return result;
    }

    /// <summary>
    /// Refreshes the user's access token using ASP.NET Core Identity.
    /// </summary>
    /// <param name="refreshToken">The user's refresh-token</param>
    /// <returns>A claims-principal containing a new token</returns>
    public async Task<ClaimsPrincipal?> RefreshAsync(string refreshToken)
    {
        var refreshTokenProtector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(refreshToken);

        // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            timeProvider.GetUtcNow() >= expiresUtc ||
            await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not ApplicationUser user)

        {
            // Signal: Challenge
            return null;
        }

        return await signInManager.CreateUserPrincipalAsync(user);
    }
}
