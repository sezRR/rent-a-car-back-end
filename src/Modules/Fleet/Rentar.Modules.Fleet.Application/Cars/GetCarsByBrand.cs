using Rentar.Shared.Application.Aspects.Caching;
using Rentar.Shared.Application.UseCases;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Cars;

public sealed record GetCarsByBrand(int BrandId);

public sealed class GetCarsByBrandHandler(ICarDal carDal) : IQueryHandler<GetCarsByBrand, List<CarDetailDto>>
{
    [CacheAspect(duration: 10)]
    public List<CarDetailDto> Handle(GetCarsByBrand query)
    {
        return carDal.GetCarDetails(c => c.BrandId == query.BrandId);
    }
}
