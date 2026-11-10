using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rentar.Shared.Infrastructure.Persistence;

internal sealed class ModuleDatabase<TContext>(IDbContextFactory<TContext> contextFactory, string schema) : IModuleDatabase where TContext : DbContext
{
    private readonly IDbContextFactory<TContext> _contextFactory = contextFactory;

    public string Schema { get; } = schema;

    public IReadOnlyList<string> GetTableNames()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Model.GetEntityTypes()
            .Where(entityType => entityType.GetTableName() != null)
            .Select(entityType => $"\"{entityType.GetSchema()}\".\"{entityType.GetTableName()}\"")
            .Distinct()
            .ToList();
    }

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await context.Database.MigrateAsync(cancellationToken);
    }
}
