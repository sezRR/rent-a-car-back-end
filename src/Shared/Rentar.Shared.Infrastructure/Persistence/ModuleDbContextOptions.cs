using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Rentar.Shared.Infrastructure.Persistence;

public static class ModuleDbContextOptions
{
    /// <summary>
    /// Points a module context at PostgreSQL and keeps its migration history inside the module's own schema,
    /// so each module migrates independently of the others.
    /// </summary>
    public static DbContextOptionsBuilder UseModuleNpgsql(this DbContextOptionsBuilder builder, string connectionString, string schema)
    {
        return builder.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema));
    }

    /// <summary>
    /// Options for <c>dotnet ef</c>, which builds a context without the host. The connection string comes
    /// from appsettings / environment variables of the working directory, falling back to the local default.
    /// </summary>
    public static DbContextOptions<TContext> ForDesignTime<TContext>(string schema) where TContext : DbContext
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var builder = new DbContextOptionsBuilder<TContext>();
        builder.UseModuleNpgsql(DatabaseConnection.Resolve(configuration), schema);
        return builder.Options;
    }
}
