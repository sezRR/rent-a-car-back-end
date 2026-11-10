using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.CarImages;

public sealed record GetCarImagesByCar(int CarId);

public sealed class GetCarImagesByCarHandler(ICarImageDal carImageDal) : IQueryHandler<GetCarImagesByCar, List<CarImage>>
{
    public List<CarImage> Handle(GetCarImagesByCar query)
    {
        return carImageDal.GetAll(c => c.CarId == query.CarId);
    }
}
