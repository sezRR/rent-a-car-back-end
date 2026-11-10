using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Contracts;

/// <summary>
/// What other modules may ask the Fleet module.
/// </summary>
public interface IFleetModuleApi
{
    /// <summary>Cars with the given ids, with their brand and color; unknown ids are left out.</summary>
    IReadOnlyList<CarSummary> GetCars(IReadOnlyCollection<int> carIds);
}
