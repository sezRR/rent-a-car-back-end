using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Fleet.Application.Cars;

/// <summary>Answers the car's details, or <c>null</c> when there is no such car or its brand or color is not stored.</summary>
public sealed record GetCarDetailsById(int Id);

public sealed class GetCarDetailsByIdHandler(ICarDal carDal) : IQueryHandler<GetCarDetailsById, CarDetailDto>
{
    public CarDetailDto Handle(GetCarDetailsById query)
    {
        return carDal.GetCarDetailsById(c => c.Id == query.Id);
    }
}
