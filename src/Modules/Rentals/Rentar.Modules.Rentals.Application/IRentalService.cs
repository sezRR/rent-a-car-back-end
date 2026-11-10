using Rentar.Modules.Rentals.Domain;
using System.Collections.Generic;

namespace Rentar.Modules.Rentals.Application;

public interface IRentalService
{
    List<Rental> GetAll();

    /// <returns>The rental, or <c>null</c> when there is none with that id.</returns>
    Rental GetById(int id);

    List<Rental> GetRentalsByCarId(int carId);

    void Add(Rental rental);
    void Update(Rental rental);
    void Delete(Rental rental);

    /// <summary>Whether the car is free for the whole period, i.e. no rental of it overlaps the period.</summary>
    bool IsRentable(Rental rental);

    /// <summary>Rentals together with their car, customer and user; a rental whose car, customer or user is gone is left out.</summary>
    /// <exception cref="Rentar.Shared.Kernel.Exceptions.UnavailableException">It is the 22:00 maintenance hour.</exception>
    List<RentalDetailDto> GetRentalDetails();

    /// <summary>The car's rental; expects at most one, as v1 does.</summary>
    Rental GetRentalByCarId(int carId);
}
