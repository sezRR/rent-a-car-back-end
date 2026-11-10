using Rentar.Modules.Rentals.Domain;
using Rentar.Shared.Kernel.Results;
using System.Collections.Generic;

namespace Rentar.Modules.Rentals.Application;

public interface IRentalService
{
    IDataResult<List<Rental>> GetAll();
    IDataResult<Rental> GetById(int id);
    IDataResult<List<Rental>> GetRentalsByCarId(int carId);

    IResult Add(Rental rental);
    IResult Update(Rental rental);
    IResult Delete(Rental rental);
    IResult IsRentable(Rental rental);

    IDataResult<List<RentalDetailDto>> GetRentalDetails();
    IDataResult<Rental> GetRentalByCarId(int carId);
}
