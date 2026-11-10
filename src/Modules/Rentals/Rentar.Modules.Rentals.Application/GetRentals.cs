using Rentar.Modules.Rentals.Domain;
using Rentar.Shared.Application.UseCases;
using System.Collections.Generic;

namespace Rentar.Modules.Rentals.Application;

public sealed record GetRentals;

public sealed class GetRentalsHandler(IRentalDal rentalDal) : IQueryHandler<GetRentals, List<Rental>>
{
    public List<Rental> Handle(GetRentals query)
    {
        return rentalDal.GetAll();
    }
}
