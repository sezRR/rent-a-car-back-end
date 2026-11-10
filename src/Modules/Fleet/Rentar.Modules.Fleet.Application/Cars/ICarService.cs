using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Kernel.Results;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Cars;

public interface ICarService
{
    IDataResult<List<Car>> GetAll();
    IDataResult<Car> GetById(int id);
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
