using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Rentar.ArchitectureTests;

/// <summary>
/// A production assembly of the solution, named <c>Rentar.Modules.{Module}.{Layer}</c>, <c>Rentar.Shared.{Layer}</c> or <c>Rentar.Api</c>.
/// </summary>
public sealed record RentarAssembly(string Name, string Module, string Layer)
{
    public const string SharedModule = "Shared";
    public const string HostModule = "Host";

    public static readonly string[] ModuleLayers = ["Contracts", "Domain", "Application", "Presentation", "Infrastructure"];

    public bool IsShared => Module == SharedModule;
    public bool IsHost => Module == HostModule;
    public bool IsModule => !IsShared && !IsHost;

    public static RentarAssembly Parse(string name)
    {
        var parts = name.Split('.');
        return parts switch
        {
            ["Rentar", "Api"] => new RentarAssembly(name, HostModule, "Api"),
            ["Rentar", "Shared", var layer] => new RentarAssembly(name, SharedModule, layer),
            ["Rentar", "Modules", var module, var layer] => new RentarAssembly(name, module, layer),
            _ => throw new ArgumentException($"'{name}' does not follow the Rentar project naming.", nameof(name))
        };
    }

    /// <summary>Every production assembly copied next to the tests.</summary>
    public static IReadOnlyList<RentarAssembly> All { get; } = Directory
        .GetFiles(AppContext.BaseDirectory, "Rentar.*.dll")
        .Select(Path.GetFileNameWithoutExtension)
        .Where(name => !name!.EndsWith("Tests", StringComparison.Ordinal))
        .Select(name => Parse(name!))
        .OrderBy(assembly => assembly.Name, StringComparer.Ordinal)
        .ToList();

    public Assembly Load()
    {
        return Assembly.Load(new AssemblyName(Name));
    }

    /// <summary>Solution assemblies this one uses. The compiler only records references that code actually uses.</summary>
    public IReadOnlyList<RentarAssembly> References()
    {
        return Load().GetReferencedAssemblies()
            .Select(reference => reference.Name!)
            .Where(name => name.StartsWith("Rentar.", StringComparison.Ordinal))
            .Select(Parse)
            .ToList();
    }

    public override string ToString() => Name;
}
