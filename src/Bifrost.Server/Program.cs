using Bifrost.Server.Data;
using Bifrost.Server.Extensions;
using Bifrost.Server.Identity;
using Bifrost.WASM.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Server;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string? baseUrls = builder.Configuration["ASPNETCORE_URLS"];
        if (string.IsNullOrWhiteSpace(baseUrls) )
        {
            throw new Exception("ASPNETCORE_URLS environment variable is not set.");
        }

        // Add blazor components
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        // SQLite identity
        var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection") ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");

        builder.Services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            o.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
            .AddBearerToken(IdentityConstants.BearerScheme)
            .AddIdentityCookies(b => b.ApplicationCookie?.Configure(o => o.LoginPath = "/Identity/Account/Login"));

        builder.Services.AddAuthorizationBuilder();

        builder.Services.AddDbContext<IdentityDbCtx>(
            options => options.UseSqlite(identityConnectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentityCore<IdentityUser>()
           .AddEntityFrameworkStores<IdentityDbCtx>()
           .AddApiEndpoints();

        builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(baseUrls.Split(';')[0]) });

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Ensure database is ready
        app.EnsureDatabaseReady<IdentityDbCtx>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGroup("/identity")
            .MapBetterIdentityApi<IdentityUser>();

        app.MapRazorComponents<App>()
            .AddAdditionalAssemblies(typeof(Counter).Assembly)
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode();

        app.Run();
    }
}
