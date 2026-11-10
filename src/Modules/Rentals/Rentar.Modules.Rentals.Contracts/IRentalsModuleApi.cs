using System;

namespace Rentar.Modules.Rentals.Contracts;

/// <summary>
/// What other modules may ask the Rentals module.
/// </summary>
public interface IRentalsModuleApi
{
    /// <summary>Whether no rental of the car overlaps the given period.</summary>
    bool IsCarAvailable(int carId, DateTime rentDate, DateTime returnDate);
}
