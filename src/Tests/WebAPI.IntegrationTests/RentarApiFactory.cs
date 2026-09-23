using Core.Utilities.IoC;
using DataAccess.Concrete.EntityFramework;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

namespace WebAPI.IntegrationTests;

/// <summary>
/// Boots the API in memory against a throwaway PostgreSQL container. The data access layer builds its
/// own DbContext instances, so the connection string is handed over through the environment variable
/// that <see cref="DbConnectionString"/> reads - it has to be set before anything touches the database.
/// </summary>
public class RentarApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string ConnectionStringVariable = "ConnectionStrings__RentarDb";

    private static readonly string[] Tables =
    {
        "Brands", "CarImages", "Cars", "Colors", "Customers",
        "OperationClaims", "Rentals", "UserOperationClaims", "Users"
    };

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:18-alpine")
        .WithDatabase("rentar_test")
        .WithUsername("rentar")
        .WithPassword("rentar")
        .Build();

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();
        Environment.SetEnvironmentVariable(ConnectionStringVariable, _postgres.GetConnectionString());

        await using var context = new RentingCarContext();
        await context.Database.MigrateAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgres.DisposeAsync();
        Environment.SetEnvironmentVariable(ConnectionStringVariable, null);
    }

    /// <summary>
    /// Empties every table and the in-process cache so each test starts from a known state.
    /// </summary>
    public async Task ResetAsync()
    {
        await using var connection = new NpgsqlConnection(_postgres.GetConnectionString());
        await connection.OpenAsync();

        var truncate = $"TRUNCATE \"{string.Join("\", \"", Tables)}\" RESTART IDENTITY CASCADE;";
        await using var command = new NpgsqlCommand(truncate, connection);
        await command.ExecuteNonQueryAsync();

        // The caching aspect keeps results in a static provider that outlives a single test.
        if (ServiceTool.ServiceProvider?.GetService<IMemoryCache>() is MemoryCache cache)
        {
            cache.Clear();
        }
    }
}
