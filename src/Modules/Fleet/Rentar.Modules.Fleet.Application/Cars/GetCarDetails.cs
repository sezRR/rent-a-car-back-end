using Rentar.Shared.Application.UseCases;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Cars;

/// <summary>Every car with the names of its brand and color; a car whose brand or color is not stored is left out.</summary>
public sealed record GetCarDetails;

public sealed class GetCarDetailsHandler(ICarDal carDal) : IQueryHandler<GetCarDetails, List<CarDetailDto>>
{
    public List<CarDetailDto> Handle(GetCarDetails query)
    {
        return carDal.GetCarDetails();
    }
}
