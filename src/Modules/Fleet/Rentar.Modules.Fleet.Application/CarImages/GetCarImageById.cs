using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Fleet.Application.CarImages;

/// <summary>Answers the image, or <c>null</c> when there is none with that id.</summary>
public sealed record GetCarImageById(int Id);

public sealed class GetCarImageByIdHandler(ICarImageDal carImageDal) : IQueryHandler<GetCarImageById, CarImage>
{
    public CarImage Handle(GetCarImageById query)
    {
        return carImageDal.Get(c => c.Id == query.Id);
    }
}
