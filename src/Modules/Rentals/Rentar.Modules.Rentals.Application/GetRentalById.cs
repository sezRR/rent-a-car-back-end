using Rentar.Modules.Rentals.Domain;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Rentals.Application;

/// <summary>Answers the rental, or <c>null</c> when there is none with that id.</summary>
public sealed record GetRentalById(int Id);

public sealed class GetRentalByIdHandler(IRentalDal rentalDal) : IQueryHandler<GetRentalById, Rental>
{
    public Rental Handle(GetRentalById query)
    {
        return rentalDal.Get(r => r.Id == query.Id);
    }
}
