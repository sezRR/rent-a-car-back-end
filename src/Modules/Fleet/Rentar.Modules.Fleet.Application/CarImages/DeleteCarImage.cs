using Rentar.Shared.Application.Aspects.Transaction;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Kernel.Exceptions;

namespace Rentar.Modules.Fleet.Application.CarImages;

/// <summary>Deletes the image and its file; throws <see cref="NotFoundException"/> when there is no such image.</summary>
public sealed record DeleteCarImage(int Id);

public sealed class DeleteCarImageHandler(ICarImageDal carImageDal, IImageStorage imageStorage) : ICommandHandler<DeleteCarImage>
{
    [TransactionScopeAspect]
    public void Handle(DeleteCarImage command)
    {
        var carImage = carImageDal.Get(c => c.Id == command.Id)
            ?? throw new NotFoundException(Messages.CarImageNotFound);

        carImageDal.Delete(carImage);
        imageStorage.Delete(carImage.ImagePath);
    }
}
