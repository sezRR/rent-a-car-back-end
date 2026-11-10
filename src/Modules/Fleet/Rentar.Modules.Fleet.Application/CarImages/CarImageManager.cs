using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Aspects.Transaction;
using Rentar.Shared.Kernel.Business;
using Rentar.Shared.Kernel.Results;
using System;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.CarImages;

public class CarImageManager(ICarImageDal carImageDal, IImageStorage imageStorage, TimeProvider timeProvider) : ICarImageService
{
    private readonly ICarImageDal _carImageDal = carImageDal;
    private readonly IImageStorage _imageStorage = imageStorage;
    private readonly TimeProvider _timeProvider = timeProvider;

    [TransactionScopeAspect]
    public IResult Add(CarImage carImage, FileUpload file)
    {
        var result = BusinessRules.Run(
            CheckIfReachCarImageLimit(carImage.CarId),
            CheckIfImageFileValid(file, allowMissing: true));

        if (result != null)
        {
            return result;
        }

        carImage.ImagePath = file == null ? IImageStorage.DefaultImageName : _imageStorage.Save(file);

        _carImageDal.Add(carImage);

        return new SuccessResult();
    }

    [TransactionScopeAspect]
    public IResult Update(CarImage carImage, FileUpload file)
    {
        var result = BusinessRules.Run(CheckIfImageFileValid(file, allowMissing: false));

        if (result != null)
        {
            return result;
        }

        // The stored path is used, not the one sent by the client, so only this image's file is replaced.
        var existing = _carImageDal.Get(c => c.Id == carImage.Id);
        if (existing == null)
        {
            return new ErrorResult(Messages.CarImageNotFound);
        }

        carImage.ImagePath = _imageStorage.Replace(file, existing.ImagePath);
        carImage.Date = _timeProvider.GetLocalNow().DateTime;

        _carImageDal.Update(carImage);

        return new SuccessResult();
    }

    [TransactionScopeAspect]
    public IResult Delete(CarImage carImage)
    {
        _carImageDal.Delete(carImage);

        _imageStorage.Delete(carImage.ImagePath);

        return new SuccessResult();
    }

    public IDataResult<List<CarImage>> GetAll()
    {
        return new SuccessDataResult<List<CarImage>>(_carImageDal.GetAll());
    }

    public IDataResult<List<CarImage>> GetByCarId(int carId)
    {
        return new SuccessDataResult<List<CarImage>>(_carImageDal.GetAll(c => c.CarId == carId));
    }

    public IDataResult<CarImage> GetById(int id)
    {
        return new SuccessDataResult<CarImage>(_carImageDal.Get(p => p.Id == id));
    }

    private IResult CheckIfReachCarImageLimit(int carId)
    {
        var countOfCarImages = _carImageDal.GetAll(c => c.CarId == carId).Count;
        if (!(countOfCarImages > 5))
        {
            return new SuccessResult();
        }
        return new ErrorResult(Messages.CarImageLimitReached);
    }

    private static IResult CheckIfImageFileValid(FileUpload file, bool allowMissing)
    {
        // Adding without a file falls back to the default image.
        if (file == null && allowMissing)
        {
            return new SuccessResult();
        }

        if (ImageFileValidator.IsSupportedImage(file))
        {
            return new SuccessResult();
        }
        return new ErrorResult(Messages.CarImageInvalidFile);
    }
}
