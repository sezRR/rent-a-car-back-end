using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Aspects.Performance;
using Rentar.Shared.Application.UseCases;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Cars;

public sealed record GetCars;

public sealed class GetCarsHandler(ICarDal carDal) : IQueryHandler<GetCars, List<Car>>
{
    [PerformanceAspect(5)]
    public List<Car> Handle(GetCars query)
    {
        return carDal.GetAll();
    }
}
