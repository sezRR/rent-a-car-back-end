using Rentar.Shared.Application.UseCases;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Cars;

public sealed record GetCarsByColor(int ColorId);

public sealed class GetCarsByColorHandler(ICarDal carDal) : IQueryHandler<GetCarsByColor, List<CarDetailDto>>
{
    public List<CarDetailDto> Handle(GetCarsByColor query)
    {
        return carDal.GetCarDetails(c => c.ColorId == query.ColorId);
    }
}
