using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class RentalManager : IRentalService
    {
        IRentalDal _rentalDal;

        public RentalManager(IRentalDal rentalDal)
        {
            _rentalDal = rentalDal;
        }

        [ValidationAspect(typeof(RentalValidator))]
        public IResult Add(Rental rental)
        {
            _rentalDal.Add(rental);

            return new SuccessResult(Messages.RentalAdded);
        }

        public IResult Delete(Rental rental)
        {
            if (rental.CarId.ToString() == null)
            {
                return new ErrorResult(Messages.CarIsInvalid);
            }
            _rentalDal.Delete(rental);

            return new SuccessResult(Messages.RentalDeleted);
        }

        public IDataResult<List<Rental>> GetAll()
        {
            //if (DateTime.Now.Hour == 22)
            //{
            //    return new ErrorDataResult<List<Rental>>(Messages.MaintenanceTime);
            //}

            return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll(), Messages.RentalsListed);
        }

        public IDataResult<Rental> GetRentalByCarId(int carId)
        {
            return new SuccessDataResult<Rental>(_rentalDal.Get(r => r.CarId == carId));
        }

        public IDataResult<List<RentalDetailDto>> GetRentalDetails()
        {
            if (DateTime.Now.Hour == 22)
            {
                return new ErrorDataResult<List<RentalDetailDto>>(Messages.MaintenanceTime);
            }

            return new SuccessDataResult<List<RentalDetailDto>>(_rentalDal.GetRentalDetails());
        }

        public IResult IsRentable(Rental rental)
        {
            var result = _rentalDal.GetAll(r => r.CarId == rental.CarId);

            if (result.Any(r =>
                r.ReturnDate >= rental.RentDate &&
                r.RentDate <= rental.ReturnDate
            )) return new ErrorResult(Messages.CarNotAvailable);

            return new SuccessResult();
        }

        public IResult Update(Rental rental)
        {
            if (rental.CarId.ToString() == null)
            {
                return new ErrorResult(Messages.CarIsInvalid);
            }
            _rentalDal.Update(rental);

            return new SuccessResult(Messages.RentalUpdated);
        }
    }
}
