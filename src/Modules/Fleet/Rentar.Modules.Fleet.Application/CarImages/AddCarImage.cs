using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Aspects.Transaction;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Kernel.Exceptions;
using System;

namespace Rentar.Modules.Fleet.Application.CarImages;

/// <summary>
/// Stores <see cref="File"/> as an image of the car, or the default image when there is no file, and answers the image
/// with its id and path set. Throws <see cref="BusinessRuleException"/> when the car already has the most images it
/// may have or the file is not a PNG or JPEG.
/// </summary>
public sealed record AddCarImage(int CarId, FileUpload File);

public sealed class AddCarImageHandler(ICarImageDal carImageDal, IImageStorage imageStorage, TimeProvider timeProvider)
    : ICommandHandler<AddCarImage, CarImage>
{
    private const int MaxImagesPerCar = 5;

    [TransactionScopeAspect]
    public CarImage Handle(AddCarImage command)
    {
        if (carImageDal.GetAll(c => c.CarId == command.CarId).Count >= MaxImagesPerCar)
        {
            throw new BusinessRuleException(Messages.CarImageLimitReached);
        }

        // Adding without a file falls back to the default image.
        if (command.File != null && !ImageFileValidator.IsSupportedImage(command.File))
        {
            throw new BusinessRuleException(Messages.CarImageInvalidFile);
        }

        var carImage = new CarImage
        {
            CarId = command.CarId,
            Date = timeProvider.GetLocalNow().DateTime,
            ImagePath = command.File == null ? IImageStorage.DefaultImageName : imageStorage.Save(command.File)
        };

        carImageDal.Add(carImage);
        return carImage;
    }
}
