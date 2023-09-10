using Bifrost.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Server;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add blazor components
        builder.Services.AddRazorComponents()
            .AddServerComponents()
            .AddWebAssemblyComponents();

        // SQLite identity
        var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection") ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");

        builder.Services.AddAuthorization();

        builder.Services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);
        builder.Services.AddAuthorizationBuilder();

        builder.Services.AddDbContext<IdentityDbCtx>(
            options => options.UseSqlite(identityConnectionString));

        builder.Services.AddIdentityCore<IdentityUser>()
           .AddEntityFrameworkStores<IdentityDbCtx>()
           .AddApiEndpoints();

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
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

        app.MapIdentityApi<IdentityUser>();

        app.UseAuthorization();

        app.MapRazorComponents<App>()
            .AddServerRenderMode()
            .AddWebAssemblyRenderMode();

        app.Run();
    }
}
