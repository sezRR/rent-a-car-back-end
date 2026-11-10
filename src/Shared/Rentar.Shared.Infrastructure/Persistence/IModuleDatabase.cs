using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rentar.Shared.Infrastructure.Persistence;

/// <summary>
/// The schema a module owns, exposed so the host and tests can migrate or inspect every module the same way.
/// </summary>
public interface IModuleDatabase
{
    string Schema { get; }

    /// <summary>Quoted, schema qualified names of the module's tables, e.g. <c>"fleet"."Cars"</c>.</summary>
    IReadOnlyList<string> GetTableNames();

    Task MigrateAsync(CancellationToken cancellationToken = default);
}
