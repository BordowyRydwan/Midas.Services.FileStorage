using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Extensions;

public static class MigrateDatabaseExtension
{
    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var fileCtx = scope.ServiceProvider.GetRequiredService<FileDbContext>();

        fileCtx.Database.Migrate();
    }
}