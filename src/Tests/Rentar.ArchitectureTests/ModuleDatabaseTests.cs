using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rentar.Shared.Infrastructure.Modules;
using Rentar.Shared.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Rentar.ArchitectureTests;

/// <summary>
/// Each module with persistent state owns one context, registered onto a schema named after the module.
/// Checked against the modules' real registrations; no database is contacted.
/// </summary>
public class ModuleDatabaseTests
{
    private static readonly string[] ModulesWithDatabase = ["Customers", "Fleet", "Identity", "Rentals"];

    private static readonly Lazy<IServiceProvider> ModuleServices = new(BuildModuleServices);

    public static TheoryData<Type> Contexts()
    {
        return new TheoryData<Type>(ContextTypes());
    }

    [Fact]
    public void ModulesWithState_HaveExactlyOneContextEach_AndStatelessModulesNone()
    {
        var modules = ContextTypes().Select(ModuleOf).ToList();

        Assert.Equal(ModulesWithDatabase, modules.Order());
    }

    [Fact]
    public void ModuleDatabases_AreRegisteredOnePerModule_WithTheirOwnSchema()
    {
        var schemas = ModuleServices.Value.GetServices<IModuleDatabase>().Select(database => database.Schema);

        Assert.Equal(ModulesWithDatabase.Select(module => module.ToLowerInvariant()), schemas.Order());
    }

    [Theory]
    [MemberData(nameof(Contexts))]
    public void Context_MapsOnlyItsOwnDomain_IntoTheModuleSchema(Type contextType)
    {
        var module = ModuleOf(contextType);
        using var context = CreateRegisteredContext(contextType);

        Assert.NotEmpty(context.Model.GetEntityTypes());
        Assert.All(context.Model.GetEntityTypes(), entityType =>
        {
            Assert.Equal(module.ToLowerInvariant(), entityType.GetSchema());
            Assert.Equal($"Rentar.Modules.{module}.Domain", entityType.ClrType.Assembly.GetName().Name);
        });
    }

    [Theory]
    [MemberData(nameof(Contexts))]
    public void Context_KeepsItsMigrationHistoryInTheModuleSchema(Type contextType)
    {
        using var context = CreateRegisteredContext(contextType);

        var options = context.GetService<IDbContextOptions>();
        Assert.Equal(ModuleOf(contextType).ToLowerInvariant(), RelationalOptionsExtension.Extract(options).MigrationsHistoryTableSchema);
    }

    private static IEnumerable<Type> ContextTypes()
    {
        return RentarAssembly.All
            .Where(assembly => assembly.IsModule)
            .SelectMany(assembly => assembly.Load().GetTypes())
            .Where(type => typeof(DbContext).IsAssignableFrom(type) && !type.IsAbstract);
    }

    private static string ModuleOf(Type type)
    {
        return RentarAssembly.Parse(type.Assembly.GetName().Name!).Module;
    }

    /// <summary>Registers every module the way the host does, minus the Autofac container.</summary>
    private static IServiceProvider BuildModuleServices()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);

        var modules = RentarAssembly.All
            .Where(assembly => assembly is { IsModule: true, Layer: "Infrastructure" })
            .SelectMany(assembly => assembly.Load().GetTypes())
            .Where(type => typeof(IModule).IsAssignableFrom(type) && !type.IsAbstract)
            .Select(type => (IModule)Activator.CreateInstance(type)!);

        foreach (var module in modules)
        {
            module.AddServices(services, configuration);
        }

        return services.BuildServiceProvider();
    }

    private static DbContext CreateRegisteredContext(Type contextType)
    {
        var factoryType = typeof(IDbContextFactory<>).MakeGenericType(contextType);
        var factory = ModuleServices.Value.GetRequiredService(factoryType);

        return (DbContext)factoryType.GetMethod(nameof(IDbContextFactory<DbContext>.CreateDbContext))!.Invoke(factory, null)!;
    }
}
