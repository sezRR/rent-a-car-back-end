using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface ICarService
    {
        IDataResult<List<Car>> GetAll();
        IDataResult<List<CarDetailDto>> GetCarsByBrandId(int brandId);
        IDataResult<List<CarDetailDto>> GetCarsByColorId(int colorId);
        IDataResult<List<CarDetailDto>> GetCarsByColorAndBrandId(int brandId, int colorId);

        IResult Add(Car car);
        IDataResult<Car> AddReturnableData(Car car);
        IResult Update(Car car);
        IResult Delete(Car car);

        IDataResult<List<CarDetailDto>> GetCarDetails();
        IDataResult<CarDetailDto> GetCarDetailsById(int id);

        IResult TransactionalOperation(Car car);
    }
}
