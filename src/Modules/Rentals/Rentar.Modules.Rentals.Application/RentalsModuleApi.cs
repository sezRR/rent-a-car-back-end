using Rentar.Modules.Rentals.Contracts;
using Rentar.Shared.Application.UseCases;
using System;

namespace Rentar.Modules.Rentals.Application;

public class RentalsModuleApi(IQueryHandler<CheckCarAvailability, bool> checkCarAvailability) : IRentalsModuleApi
{
    public bool IsCarAvailable(int carId, DateTime rentDate, DateTime returnDate)
    {
        return checkCarAvailability.Handle(new CheckCarAvailability(carId, rentDate, returnDate));
    }
}
