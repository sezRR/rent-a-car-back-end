using Rentar.Modules.Rentals.Domain;
using Rentar.Shared.Application.UseCases;
using System.Collections.Generic;

namespace Rentar.Modules.Rentals.Application;

/// <summary>Every rental of the Fleet car <see cref="CarId"/>.</summary>
public sealed record GetRentalsByCar(int CarId);

public sealed class GetRentalsByCarHandler(IRentalDal rentalDal) : IQueryHandler<GetRentalsByCar, List<Rental>>
{
    public List<Rental> Handle(GetRentalsByCar query)
    {
        return rentalDal.GetAll(r => r.CarId == query.CarId);
    }
}
