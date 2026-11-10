using Microsoft.Extensions.Configuration;

namespace Rentar.Shared.Infrastructure.Persistence;

/// <summary>
/// Every module keeps its tables in its own schema of one PostgreSQL database, reached through one connection string.
/// </summary>
public static class DatabaseConnection
{
    public const string ConnectionStringName = "RentarDb";

    // Matches the defaults in docker-compose.yml, so migrations run without extra configuration.
    public const string LocalDefault = "Host=localhost;Port=5432;Database=rentar;Username=rentar;Password=rentar";

    public static string Resolve(IConfiguration configuration)
    {
        return configuration.GetConnectionString(ConnectionStringName) ?? LocalDefault;
    }
}
