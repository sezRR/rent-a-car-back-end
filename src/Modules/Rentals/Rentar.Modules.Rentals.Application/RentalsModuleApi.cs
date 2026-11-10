using Rentar.Modules.Rentals.Contracts;
using Rentar.Modules.Rentals.Domain;
using System;

namespace Rentar.Modules.Rentals.Application;

public class RentalsModuleApi(IRentalService rentalService) : IRentalsModuleApi
{
    public bool IsCarAvailable(int carId, DateTime rentDate, DateTime returnDate)
    {
        return rentalService.IsRentable(new Rental { CarId = carId, RentDate = rentDate, ReturnDate = returnDate }).Success;
    }
}
