using Autofac;
using Rentar.Shared.Infrastructure.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Rentar.ArchitectureTests;

/// <summary>
/// Application logic lives in use cases: a record describing the use case and one sealed handler for it, next to
/// each other. No service class is left to gather unrelated operations.
/// </summary>
public class UseCaseTests
{
    private static readonly Lazy<IContainer> ModuleContainer = new(BuildModuleContainer);

    public static TheoryData<string> ApplicationAssemblies()
    {
        return new TheoryData<string>(ApplicationLayers().Select(assembly => assembly.Name));
    }

    [Theory]
    [MemberData(nameof(ApplicationAssemblies))]
    public void ApplicationLayer_HasNoManagersOrServices(string assemblyName)
    {
        var leftovers = RentarAssembly.Parse(assemblyName).Load().GetTypes()
            .Where(type => type.Name.EndsWith("Manager", StringComparison.Ordinal)
                || (type.IsInterface && type.Name.StartsWith('I') && type.Name.EndsWith("Service", StringComparison.Ordinal)))
            .Select(type => type.FullName);

        Assert.Empty(leftovers);
    }

    [Theory]
    [MemberData(nameof(ApplicationAssemblies))]
    public void EveryHandler_IsSealed_AndHandlesOneRecordNamedAfterIt(string assemblyName)
    {
        var problems = new List<string>();

        foreach (var handler in HandlersIn(RentarAssembly.Parse(assemblyName)))
        {
            var handled = UseCaseRegistrationExtensions.HandledInterfacesOf(handler).ToList();
            if (handled.Count != 1)
            {
                problems.Add($"{handler.Name} handles {handled.Count} use cases instead of one");
                continue;
            }

            var useCase = handled[0].GetGenericArguments()[0];

            if (!handler.IsSealed)
            {
                problems.Add($"{handler.Name} is not sealed");
            }

            if (useCase.GetMethod("<Clone>$") is null)
            {
                problems.Add($"{useCase.Name}, handled by {handler.Name}, is not a record");
            }

            if (handler.Name != $"{useCase.Name}Handler" || handler.Namespace != useCase.Namespace)
            {
                problems.Add($"{handler.FullName} should be {useCase.Namespace}.{useCase.Name}Handler");
            }
        }

        Assert.Empty(problems);
    }

    [Fact]
    public void EveryUseCase_HasExactlyOneHandler()
    {
        var shared = ApplicationLayers()
            .SelectMany(HandlersIn)
            .SelectMany(handler => UseCaseRegistrationExtensions.HandledInterfacesOf(handler)
                .Select(handled => (UseCase: handled.GetGenericArguments()[0], Handler: handler)))
            .GroupBy(pair => pair.UseCase)
            .Where(group => group.Count() > 1)
            .Select(group => $"{group.Key.Name}: {string.Join(", ", group.Select(pair => pair.Handler.Name))}");

        Assert.Empty(shared);
    }

    [Fact]
    public void EveryHandler_IsRegisteredByItsModule()
    {
        var missing = ApplicationLayers()
            .SelectMany(HandlersIn)
            .SelectMany(UseCaseRegistrationExtensions.HandledInterfacesOf)
            .Where(handled => !ModuleContainer.Value.IsRegistered(handled))
            .Select(handled => handled.ToString());

        Assert.Empty(missing);
    }

    private static IEnumerable<RentarAssembly> ApplicationLayers()
    {
        return RentarAssembly.All.Where(assembly => assembly is { IsModule: true, Layer: "Application" });
    }

    private static IEnumerable<Type> HandlersIn(RentarAssembly assembly)
    {
        return assembly.Load().GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && UseCaseRegistrationExtensions.HandledInterfacesOf(type).Any());
    }

    /// <summary>Registers every module's container setup the way the host does. Nothing is resolved.</summary>
    private static IContainer BuildModuleContainer()
    {
        var builder = new ContainerBuilder();

        var modules = RentarAssembly.All
            .Where(assembly => assembly is { IsModule: true, Layer: "Infrastructure" })
            .SelectMany(assembly => assembly.Load().GetTypes())
            .Where(type => typeof(IModule).IsAssignableFrom(type) && !type.IsAbstract)
            .Select(type => (IModule)Activator.CreateInstance(type)!);

        foreach (var module in modules)
        {
            module.ConfigureContainer(builder);
        }

        return builder.Build();
    }
}
