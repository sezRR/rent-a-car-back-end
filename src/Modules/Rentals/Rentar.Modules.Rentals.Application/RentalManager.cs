using Rentar.Modules.Customers.Contracts;
using Rentar.Modules.Fleet.Contracts;
using Rentar.Modules.Identity.Contracts;
using Rentar.Modules.Rentals.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Kernel.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rentar.Modules.Rentals.Application;

public class RentalManager(
    IRentalDal rentalDal,
    ICustomersModuleApi customers,
    IIdentityModuleApi identity,
    IFleetModuleApi fleet,
    TimeProvider timeProvider) : IRentalService
{
    private readonly IRentalDal _rentalDal = rentalDal;
    private readonly ICustomersModuleApi _customers = customers;
    private readonly IIdentityModuleApi _identity = identity;
    private readonly IFleetModuleApi _fleet = fleet;
    private readonly TimeProvider _timeProvider = timeProvider;

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
        return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll(), Messages.RentalsListed);
    }

    public IDataResult<Rental> GetById(int id)
    {
        return new SuccessDataResult<Rental>(_rentalDal.Get(r => r.Id == id));
    }

    public IDataResult<List<Rental>> GetRentalsByCarId(int carId)
    {
        return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll(r => r.CarId == carId), Messages.RentalsListed);
    }

    public IDataResult<Rental> GetRentalByCarId(int carId)
    {
        return new SuccessDataResult<Rental>(_rentalDal.Get(r => r.CarId == carId));
    }

    public IDataResult<List<RentalDetailDto>> GetRentalDetails()
    {
        if (_timeProvider.GetLocalNow().Hour == 22)
        {
            return new ErrorDataResult<List<RentalDetailDto>>(Messages.MaintenanceTime);
        }

        var rentals = _rentalDal.GetAll();

        // A rental only holds the ids of other modules' records, so their details come through those modules' public APIs.
        var customers = _customers.GetCustomers(rentals.Select(r => r.CustomerId).Distinct().ToList())
            .ToDictionary(c => c.CustomerId);
        var users = _identity.GetUsers(customers.Values.Select(c => c.UserId).Distinct().ToList())
            .ToDictionary(u => u.UserId);
        var cars = _fleet.GetCars(rentals.Select(r => r.CarId).Distinct().ToList())
            .ToDictionary(c => c.CarId);

        var details = new List<RentalDetailDto>();
        foreach (var rental in rentals)
        {
            // A rental is listed only while its customer, the customer's user and the car all still exist.
            if (!customers.TryGetValue(rental.CustomerId, out var customer)
                || !users.TryGetValue(customer.UserId, out var user)
                || !cars.TryGetValue(rental.CarId, out var car))
            {
                continue;
            }

            details.Add(new RentalDetailDto
            {
                RentalId = rental.Id,
                CarId = rental.CarId,
                RentDate = rental.RentDate,
                ReturnDate = rental.ReturnDate,
                BrandName = car.BrandName,
                ColorName = car.ColorName,
                Description = car.Description,
                DailyPrice = car.DailyPrice,
                ModelYear = car.ModelYear,
                CompanyName = customer.CompanyName,
                UserName = user.FirstName
            });
        }

        return new SuccessDataResult<List<RentalDetailDto>>(details);
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
