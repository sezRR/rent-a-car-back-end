﻿using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.FileHelper;
using Core.Utilities.FileOperations;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Business.Concrete
{
    public class CarImageManager : ICarImageService
    {
        ICarImageDal _carImageDal;

        public CarImageManager(ICarImageDal carImageDal)
        {
            _carImageDal = carImageDal;
        }

        //[ValidationAspect(typeof(CarImageValidator))]
        public IResult Add(CarImage carImage, IFormFile file, string path)
        {
            var result = BusinessRules.Run(CheckIfReachCarImageLimit(carImage.CarId));

            if (result != null)
            {
                return result;
            }

            carImage.ImagePath = FileOperations.CheckCarImageDefaultOrNot(file, path).Data;

            _carImageDal.Add(carImage);

            return new SuccessResult();
        }

        [ValidationAspect(typeof(CarImageValidator))]
        public IResult Update(CarImage carImage, IFormFile file, string path)
        {
            carImage.ImagePath = FileHelper.Update(file, carImage.ImagePath, path);
            carImage.Date = DateTime.Now;

            _carImageDal.Update(carImage);

            return new SuccessResult();
        }

        public IResult Delete(CarImage carImage, string path)
        {
            if (carImage.ImagePath == "default.png")
            {
                _carImageDal.Delete(carImage);
            }
            else
            {
                FileHelper.Delete(carImage.ImagePath, path);

                _carImageDal.Delete(carImage);
            }

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

        private List<CarImage> CheckIfCarImageNull(int carId)
        {
            string imagePath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).FullName + @"\CarImages\default-image.jpg");
            var result = _carImageDal.GetAll(c => c.CarId == carId).Any();

            if (!result)
            {
                return new List<CarImage> { new CarImage { CarId = carId, ImagePath = imagePath, Date = DateTime.Now } };
            }

            return _carImageDal.GetAll(c => c.CarId == carId);
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
    }
}
