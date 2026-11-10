using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rentar.Shared.Infrastructure.Persistence;

public static class PersistenceServiceCollectionExtensions
{
    /// <summary>
    /// Registers a module's own context, bound to its own schema. The connection string is read when the
    /// first context is created, so configuration added late (for example by tests) still applies.
    /// </summary>
    public static IServiceCollection AddModuleDbContext<TContext>(this IServiceCollection services, string schema)
        where TContext : DbContext
    {
        services.AddDbContextFactory<TContext>((serviceProvider, options) =>
            options.UseModuleNpgsql(DatabaseConnection.Resolve(serviceProvider.GetRequiredService<IConfiguration>()), schema));

        services.AddSingleton<IModuleDatabase>(serviceProvider =>
            new ModuleDatabase<TContext>(serviceProvider.GetRequiredService<IDbContextFactory<TContext>>(), schema));

        return services;
    }

    /// <summary>
    /// Applies the pending migrations of every module, one schema after the other.
    /// </summary>
    public static async Task MigrateModuleDatabasesAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        foreach (var database in serviceProvider.GetServices<IModuleDatabase>().OrderBy(d => d.Schema))
        {
            await database.MigrateAsync(cancellationToken);
        }
    }
}
