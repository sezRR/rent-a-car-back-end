using Rentar.Shared.Application.UseCases;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Cars;

public sealed record GetCarsByBrandAndColor(int BrandId, int ColorId);

public sealed class GetCarsByBrandAndColorHandler(ICarDal carDal) : IQueryHandler<GetCarsByBrandAndColor, List<CarDetailDto>>
{
    public List<CarDetailDto> Handle(GetCarsByBrandAndColor query)
    {
        return carDal.GetCarDetails(c => c.BrandId == query.BrandId && c.ColorId == query.ColorId);
    }
}
