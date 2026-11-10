using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Aspects.Transaction;
using Rentar.Shared.Kernel.Exceptions;
using System;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.CarImages;

public class CarImageManager(ICarImageDal carImageDal, IImageStorage imageStorage, TimeProvider timeProvider) : ICarImageService
{
    private readonly ICarImageDal _carImageDal = carImageDal;
    private readonly IImageStorage _imageStorage = imageStorage;
    private readonly TimeProvider _timeProvider = timeProvider;

    [TransactionScopeAspect]
    public void Add(CarImage carImage, FileUpload file)
    {
        EnsureCarImageLimitNotReached(carImage.CarId);
        EnsureImageFileIsValid(file, allowMissing: true);

        carImage.ImagePath = file == null ? IImageStorage.DefaultImageName : _imageStorage.Save(file);

        _carImageDal.Add(carImage);
    }

    [TransactionScopeAspect]
    public void Update(CarImage carImage, FileUpload file)
    {
        EnsureImageFileIsValid(file, allowMissing: false);

        // The stored path is used, not the one sent by the client, so only this image's file is replaced.
        var existing = _carImageDal.Get(c => c.Id == carImage.Id)
            ?? throw new NotFoundException(Messages.CarImageNotFound);

        carImage.ImagePath = _imageStorage.Replace(file, existing.ImagePath);
        carImage.Date = _timeProvider.GetLocalNow().DateTime;

        _carImageDal.Update(carImage);
    }

    [TransactionScopeAspect]
    public void Delete(CarImage carImage)
    {
        _carImageDal.Delete(carImage);

        _imageStorage.Delete(carImage.ImagePath);
    }

    public List<CarImage> GetAll()
    {
        return _carImageDal.GetAll();
    }

    public List<CarImage> GetByCarId(int carId)
    {
        return _carImageDal.GetAll(c => c.CarId == carId);
    }

    public CarImage GetById(int id)
    {
        return _carImageDal.Get(p => p.Id == id);
    }

    private void EnsureCarImageLimitNotReached(int carId)
    {
        var countOfCarImages = _carImageDal.GetAll(c => c.CarId == carId).Count;
        if (countOfCarImages > 5)
        {
            throw new BusinessRuleException(Messages.CarImageLimitReached);
        }
    }

    private static void EnsureImageFileIsValid(FileUpload file, bool allowMissing)
    {
        // Adding without a file falls back to the default image.
        if (file == null && allowMissing)
        {
            return;
        }

        if (!ImageFileValidator.IsSupportedImage(file))
        {
            throw new BusinessRuleException(Messages.CarImageInvalidFile);
        }
    }
}
