using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.CarImages;

/// <summary>The images of every car.</summary>
public sealed record GetCarImages;

public sealed class GetCarImagesHandler(ICarImageDal carImageDal) : IQueryHandler<GetCarImages, List<CarImage>>
{
    public List<CarImage> Handle(GetCarImages query)
    {
        return carImageDal.GetAll();
    }
}
