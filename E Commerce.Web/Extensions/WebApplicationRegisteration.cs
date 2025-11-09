using E_Commerce.Domain.Contracts;
using E_Commerce.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace E_Commerce.Web.Extensions
{
    public static class WebApplicationRegisteration
    {
        public static WebApplication MigrateDatabase(this WebApplication app)
        {
            using var Scope = app.Services.CreateScope();
            var DbContextService = Scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            if (DbContextService.Database.GetPendingMigrations().Any())
                DbContextService.Database.Migrate();
            return app;
        }

        public static WebApplication SeedDatabase(this WebApplication app)
        {

            using var Scope = app.Services.CreateScope();
            var DataIntializerService = Scope.ServiceProvider.GetRequiredService<IDataIntializer>();
            DataIntializerService.Intialize();
            return app;
        }
    }
}
