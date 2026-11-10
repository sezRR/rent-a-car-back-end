using Rentar.Shared.Application.UseCases;
using System;
using System.Linq;

namespace Rentar.Modules.Rentals.Application;

/// <summary>Whether the car is free for the whole period, i.e. no rental of it overlaps the period.</summary>
public sealed record CheckCarAvailability(int CarId, DateTime RentDate, DateTime ReturnDate);

public sealed class CheckCarAvailabilityHandler(IRentalDal rentalDal) : IQueryHandler<CheckCarAvailability, bool>
{
    public bool Handle(CheckCarAvailability query)
    {
        return !rentalDal.GetAll(r => r.CarId == query.CarId)
            .Any(r => r.ReturnDate >= query.RentDate && r.RentDate <= query.ReturnDate);
    }
}
