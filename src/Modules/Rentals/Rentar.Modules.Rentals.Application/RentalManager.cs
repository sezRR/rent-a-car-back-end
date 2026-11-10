using Rentar.Modules.Customers.Contracts;
using Rentar.Modules.Fleet.Contracts;
using Rentar.Modules.Identity.Contracts;
using Rentar.Modules.Rentals.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Kernel.Exceptions;
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
    public void Add(Rental rental)
    {
        _rentalDal.Add(rental);
    }

    public void Delete(Rental rental)
    {
        _rentalDal.Delete(rental);
    }

    public List<Rental> GetAll()
    {
        return _rentalDal.GetAll();
    }

    public Rental GetById(int id)
    {
        return _rentalDal.Get(r => r.Id == id);
    }

    public List<Rental> GetRentalsByCarId(int carId)
    {
        return _rentalDal.GetAll(r => r.CarId == carId);
    }

    public Rental GetRentalByCarId(int carId)
    {
        return _rentalDal.Get(r => r.CarId == carId);
    }

    public List<RentalDetailDto> GetRentalDetails()
    {
        if (_timeProvider.GetLocalNow().Hour == 22)
        {
            throw new UnavailableException(Messages.MaintenanceTime);
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

        return details;
    }

    public bool IsRentable(Rental rental)
    {
        return !_rentalDal.GetAll(r => r.CarId == rental.CarId)
            .Any(r => r.ReturnDate >= rental.RentDate && r.RentDate <= rental.ReturnDate);
    }

    public void Update(Rental rental)
    {
        _rentalDal.Update(rental);
    }
}
