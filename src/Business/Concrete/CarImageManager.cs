using Business.Abstract;
using Business.Constants;
using Core.Utilities.Business;
using Core.Utilities.FileHelper;
using Core.Aspects.Autofac.Transaction;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using IResult = Core.Utilities.Results.IResult;

namespace Business.Concrete;

public class CarImageManager : ICarImageService
{
    private readonly ICarImageDal _carImageDal;

    public CarImageManager(ICarImageDal carImageDal)
    {
        _carImageDal = carImageDal;
    }

    //[ValidationAspect(typeof(CarImageValidator))]
    [TransactionScopeAspect]
    public IResult Add(CarImage carImage, IFormFile file)
    {
        var result = BusinessRules.Run(
            CheckIfReachCarImageLimit(carImage.CarId),
            CheckIfImageFileValid(file, allowMissing: true));

        if (result != null)
        {
            return result;
        }

        carImage.ImagePath = file == null ? FileHelper.DefaultImageName : FileHelper.Add(file);

        _carImageDal.Add(carImage);

        return new SuccessResult();
    }

    //[ValidationAspect(typeof(CarImageValidator))]
    [TransactionScopeAspect]
    public IResult Update(CarImage carImage, IFormFile file)
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

        carImage.ImagePath = FileHelper.Update(file, existing.ImagePath);
        carImage.Date = DateTime.Now;

        _carImageDal.Update(carImage);

        return new SuccessResult();
    }

    [TransactionScopeAspect]
    public IResult Delete(CarImage carImage)
    {
        _carImageDal.Delete(carImage);

        FileHelper.Delete(carImage.ImagePath);

        return new SuccessResult();
    }

    public IDataResult<List<CarImage>> GetAll()
    {
        return new SuccessDataResult<List<CarImage>>(_carImageDal.GetAll());
    }

    public IDataResult<List<CarImage>> GetByCarId(int carId)
    {
        //return new SuccessDataResult<List<CarImage>>(CheckIfCarImageNull(carId));
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

    private static IResult CheckIfImageFileValid(IFormFile file, bool allowMissing)
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
