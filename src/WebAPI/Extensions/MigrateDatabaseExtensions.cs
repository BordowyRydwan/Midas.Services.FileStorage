using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Extensions;

public static class MigrateDatabaseExtension
{
    public static void MigrateDatabase(this WebApplication app)
    {
        var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<FileDbContext>();

        if (ctx.Database.GetPendingMigrations().Any()) {
            ctx.Database.Migrate();
        }
    }
}