using Bifrost.Client.Features.Identity.Actions;
using Bifrost.Components;
using Bifrost.Components.Account;
using Bifrost.Data;
using Bifrost.Features.Identity;
using Bifrost.Features.Identity.Actions;
using Bifrost.Features.Identity.Model;
using Bifrost.Features.PortalDefinitions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bifrost;
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHttpContextAccessor();

        // Render-Modes
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        // DbContext
        var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection") ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(identityConnectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // Fake EmailSender
        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Features
        builder.Services.AddIdentityFeature();
        builder.Services.AddPortalFeature();

        // Server-Side-Requests
        builder.Services.AddScoped<ILoginAction, ServersideLoginAction>();

        var app = builder.Build();

        // Ensure database is ready
        app.EnsureDatabaseReady<ApplicationDbContext>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
            app.UseMigrationsEndPoint();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        // Has to be called after UseAuthentication
        app.UseAntiforgery();

        // Features
        app.MapIdentityFeature();
        app.MapPortalFeature();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

        app.Run();
    }
}
