using Rentar.Modules.Fleet.Application.Cars;
using Rentar.Modules.Fleet.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Rentar.Modules.Fleet.Application;

public class FleetModuleApi(ICarDal carDal) : IFleetModuleApi
{
    private readonly ICarDal _carDal = carDal;

    public IReadOnlyList<CarSummary> GetCars(IReadOnlyCollection<int> carIds)
    {
        if (carIds.Count == 0)
        {
            return [];
        }

        var ids = carIds.ToArray();
        return _carDal.GetCarDetails(c => ids.Contains(c.Id))
            .Select(c => new CarSummary(c.CarId, c.BrandName, c.ColorName, c.ModelYear, c.DailyPrice, c.Description))
            .ToList();
    }
}
