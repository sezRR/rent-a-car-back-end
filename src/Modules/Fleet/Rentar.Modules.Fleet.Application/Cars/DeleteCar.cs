using Rentar.Modules.Fleet.Application.CarImages;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Aspects.Transaction;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Fleet.Application.Cars;

/// <summary>Deletes the car together with its images and their files, all or nothing.</summary>
public sealed record DeleteCar(int Id);

public sealed class DeleteCarHandler(ICarDal carDal, ICarImageDal carImageDal, IImageStorage imageStorage) : ICommandHandler<DeleteCar>
{
    [TransactionScopeAspect]
    public void Handle(DeleteCar command)
    {
        // Images are not tied to the car by a foreign key, so their rows and files are removed here. The file
        // storage takes part in the transaction, so a failure further on brings the files back too.
        foreach (var carImage in carImageDal.GetAll(c => c.CarId == command.Id))
        {
            carImageDal.Delete(carImage);
            imageStorage.Delete(carImage.ImagePath);
        }

        carDal.Delete(new Car { Id = command.Id });
    }
}
