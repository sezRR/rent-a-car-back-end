using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Aspects.Transaction;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Kernel.Exceptions;
using System;

namespace Rentar.Modules.Fleet.Application.CarImages;

/// <summary>
/// Replaces the stored file of image <see cref="Id"/> with <see cref="File"/>. Throws
/// <see cref="BusinessRuleException"/> when the file is missing or not a PNG or JPEG, and
/// <see cref="NotFoundException"/> when there is no such image.
/// </summary>
public sealed record ReplaceCarImage(int Id, int CarId, FileUpload File);

public sealed class ReplaceCarImageHandler(ICarImageDal carImageDal, IImageStorage imageStorage, TimeProvider timeProvider)
    : ICommandHandler<ReplaceCarImage>
{
    [TransactionScopeAspect]
    public void Handle(ReplaceCarImage command)
    {
        if (!ImageFileValidator.IsSupportedImage(command.File))
        {
            throw new BusinessRuleException(Messages.CarImageInvalidFile);
        }

        // The stored path is used, not one sent by the client, so only this image's file is replaced.
        var existing = carImageDal.Get(c => c.Id == command.Id)
            ?? throw new NotFoundException(Messages.CarImageNotFound);

        carImageDal.Update(new CarImage
        {
            Id = command.Id,
            CarId = command.CarId,
            ImagePath = imageStorage.Replace(command.File, existing.ImagePath),
            Date = timeProvider.GetLocalNow().DateTime
        });
    }
}
