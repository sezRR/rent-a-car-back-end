using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Rentar.Shared.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

namespace Rentar.Api.IntegrationTests;

/// <summary>
/// Boots the API in memory against a throwaway PostgreSQL container, with every module's schema migrated.
/// </summary>
public class RentarApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:18-alpine")
        .WithDatabase("rentar_test")
        .WithUsername("rentar")
        .WithPassword("rentar")
        .Build();

    private IReadOnlyList<string> _tables = [];

    public TestClock Clock { get; } = new();

    public string ConnectionString => _postgres.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();

        // Touching Services builds the host, which picks up the container's connection string below.
        await Services.MigrateModuleDatabasesAsync();

        _tables = Services.GetServices<IModuleDatabase>()
            .SelectMany(database => database.GetTableNames())
            .ToList();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting($"ConnectionStrings:{DatabaseConnection.ConnectionStringName}", ConnectionString);
        builder.UseSetting("Database:MigrateOnStartup", "false");

        builder.ConfigureTestServices(services => services.AddSingleton<TimeProvider>(Clock));
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    /// <summary>
    /// Empties every module's tables, the in-process cache and the clock so each test starts from a known state.
    /// </summary>
    public async Task ResetAsync()
    {
        Clock.Reset();

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        var truncate = $"TRUNCATE {string.Join(", ", _tables)} RESTART IDENTITY CASCADE;";
        await using var command = new NpgsqlCommand(truncate, connection);
        await command.ExecuteNonQueryAsync();

        // The caching aspect keeps results in the singleton cache, which outlives a single test.
        if (Services.GetService<IMemoryCache>() is MemoryCache cache)
        {
            cache.Clear();
        }
    }
}
