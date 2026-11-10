using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Fleet.Application.Cars;

/// <summary>Answers the car, or <c>null</c> when there is none with that id.</summary>
public sealed record GetCarById(int Id);

public sealed class GetCarByIdHandler(ICarDal carDal) : IQueryHandler<GetCarById, Car>
{
    public Car Handle(GetCarById query)
    {
        return carDal.Get(c => c.Id == query.Id);
    }
}
