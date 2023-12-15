using Microsoft.EntityFrameworkCore;

namespace Bifrost.Data;

public static class EnsureDatabaseExtension
{
    public static void EnsureDatabaseReady<T>(this WebApplication app) where T : DbContext
    {
        using var serviceScope = app.Services.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<T>();
        context.Database.Migrate();
    }
}
