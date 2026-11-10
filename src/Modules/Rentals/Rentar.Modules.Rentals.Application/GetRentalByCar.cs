using Rentar.Modules.Rentals.Domain;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Rentals.Application;

/// <summary>
/// The one rental of the Fleet car <see cref="CarId"/>, or <c>null</c>, as v1 asks for it. It fails when the car has
/// more than one rental; <see cref="GetRentalsByCar"/> lists them all.
/// </summary>
public sealed record GetRentalByCar(int CarId);

public sealed class GetRentalByCarHandler(IRentalDal rentalDal) : IQueryHandler<GetRentalByCar, Rental>
{
    public Rental Handle(GetRentalByCar query)
    {
        return rentalDal.Get(r => r.CarId == query.CarId);
    }
}
