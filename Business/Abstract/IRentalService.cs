using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IRentalService
    {
        IDataResult<List<Rental>> GetAll();

        IResult Add(Rental rental);
        IResult Update(Rental rental);
        IResult Delete(Rental rental);
        IResult IsRentable(Rental rental);

        IDataResult<List<RentalDetailDto>> GetRentalDetails();
        IDataResult<Rental> GetRentalByCarId(int carId);
    }
}
