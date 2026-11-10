using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Rentar.ArchitectureTests;

/// <summary>
/// Every layer is its own assembly, so the module boundaries and layer rules are checked on assembly references.
/// </summary>
public class ModuleBoundaryTests
{
    private static readonly string[] ExpectedModules = ["Customers", "Findeks", "Fleet", "Identity", "Payments", "Rentals"];

    /// <summary>Layers of its own module each module layer may use.</summary>
    private static readonly Dictionary<string, string[]> OwnModuleLayers = new()
    {
        ["Contracts"] = [],
        ["Domain"] = [],
        ["Application"] = ["Domain", "Contracts"],
        ["Presentation"] = ["Application", "Domain", "Contracts"],
        ["Infrastructure"] = ["Application", "Domain", "Contracts", "Presentation"]
    };

    /// <summary>Shared projects each module layer may use.</summary>
    private static readonly Dictionary<string, string[]> SharedLayers = new()
    {
        ["Contracts"] = ["Kernel"],
        ["Domain"] = ["Kernel"],
        ["Application"] = ["Kernel", "Application"],
        // Endpoints call use cases through the handler interfaces in Shared.Application, as they use their own Application.
        ["Presentation"] = ["Kernel", "Application", "Presentation"],
        ["Infrastructure"] = ["Kernel", "Application", "Infrastructure", "Presentation"]
    };

    /// <summary>Shared projects each shared project may use. Shared code never knows about a module.</summary>
    private static readonly Dictionary<string, string[]> SharedToShared = new()
    {
        ["Kernel"] = [],
        ["Application"] = ["Kernel"],
        ["Presentation"] = ["Kernel"],
        ["Infrastructure"] = ["Kernel", "Application"]
    };

    public static TheoryData<string> Assemblies()
    {
        return new TheoryData<string>(RentarAssembly.All.Select(assembly => assembly.Name));
    }

    [Fact]
    public void EveryModule_HasAllFiveLayers()
    {
        var layersByModule = RentarAssembly.All
            .Where(assembly => assembly.IsModule)
            .GroupBy(assembly => assembly.Module)
            .ToDictionary(group => group.Key, group => group.Select(assembly => assembly.Layer).Order().ToArray());

        Assert.Equal(ExpectedModules, layersByModule.Keys.Order());
        Assert.All(layersByModule, module => Assert.Equal(RentarAssembly.ModuleLayers.Order(), module.Value));
    }

    [Theory]
    [MemberData(nameof(Assemblies))]
    public void Assembly_UsesOnlyWhatItsLayerAllows(string assemblyName)
    {
        var assembly = RentarAssembly.Parse(assemblyName);

        var violations = assembly.References()
            .Where(reference => !IsAllowed(assembly, reference))
            .Select(reference => reference.Name)
            .ToList();

        Assert.True(violations.Count == 0, $"{assembly} must not use: {string.Join(", ", violations)}");
    }

    private static bool IsAllowed(RentarAssembly from, RentarAssembly to)
    {
        if (from.IsHost)
        {
            // The host composes modules through their Infrastructure (composition root) only.
            return to.IsShared || (to.IsModule && to.Layer == "Infrastructure");
        }

        if (from.IsShared)
        {
            return to.IsShared && SharedToShared[from.Layer].Contains(to.Layer);
        }

        if (to.IsHost)
        {
            return false;
        }

        if (to.IsShared)
        {
            return SharedLayers[from.Layer].Contains(to.Layer);
        }

        if (to.Module == from.Module)
        {
            return OwnModuleLayers[from.Layer].Contains(to.Layer);
        }

        // Across modules only the public contracts may be used, and only by application logic.
        return from.Layer == "Application" && to.Layer == "Contracts";
    }
}
