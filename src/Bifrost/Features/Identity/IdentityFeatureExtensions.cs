using Bifrost.Components.Account;
using Bifrost.Data;
using Bifrost.Features.Identity.Model;
using Bifrost.Features.Identity.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Bifrost.Features.Identity;

public static class IdentityFeatureExtensions
{
    public static void AddIdentityFeature(this IServiceCollection services)
    {
        services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

        services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddApiEndpoints();

        // Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
        })
            .AddBearerToken(IdentityConstants.BearerScheme)
            .AddIdentityCookies(b => b.ApplicationCookie!.Configure(o => o.LoginPath = "/Identity/Account/Login"));

        // Authorization
        services.AddCascadingAuthenticationState();
        services.AddAuthorizationBuilder()
            .AddPolicy("ApiPolicy", p => p
                .AddAuthenticationSchemes(IdentityConstants.BearerScheme)
                .RequireAuthenticatedUser());

        // Services
        services.AddScoped<IIdentityService, IdentityService>();
    }

    public static void MapIdentityFeature(this WebApplication app)
    {
        app.MapGroup("/identity")
            .MapIdentityApiWithUsername<IdentityUser>();
    }
}
