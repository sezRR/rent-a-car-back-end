using Rentar.Shared.Application.Aspects.Transaction;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Fleet.Application.CarImages;

/// <summary>Deletes every image of the car and their files, all or nothing. A car without images is left as it is.</summary>
public sealed record DeleteCarImagesOfCar(int CarId);

public sealed class DeleteCarImagesOfCarHandler(ICarImageDal carImageDal, IImageStorage imageStorage) : ICommandHandler<DeleteCarImagesOfCar>
{
    [TransactionScopeAspect]
    public void Handle(DeleteCarImagesOfCar command)
    {
        foreach (var carImage in carImageDal.GetAll(c => c.CarId == command.CarId))
        {
            carImageDal.Delete(carImage);
            imageStorage.Delete(carImage.ImagePath);
        }
    }
}
