using Rentar.Modules.Fleet.Application.CarImages;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Aspects.Caching;
using Rentar.Shared.Application.Aspects.Performance;
using Rentar.Shared.Application.Aspects.Transaction;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Kernel.Results;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Cars;

public class CarManager(ICarDal carDal, ICarImageService carImageService) : ICarService
{
    private readonly ICarDal _carDal = carDal;
    private readonly ICarImageService _carImageService = carImageService;

    [ValidationAspect(typeof(CarValidator))]
    [CacheRemoveAspect("ICarService.Get")]
    public IResult Add(Car car)
    {
        _carDal.Add(car);

        return new SuccessResult(Messages.CarAdded);
    }

    public IResult Update(Car car)
    {
        _carDal.Update(car);

        return new SuccessResult(Messages.CarUpdated);
    }

    [TransactionScopeAspect]
    public IResult Delete(Car car)
    {
        // Images are not tied to the car by a foreign key, so their rows and files are removed here.
        foreach (var carImage in _carImageService.GetByCarId(car.Id).Data)
        {
            _carImageService.Delete(carImage);
        }

        _carDal.Delete(car);

        return new SuccessResult(Messages.CarDeleted);
    }

    [PerformanceAspect(5)]
    public IDataResult<List<Car>> GetAll()
    {
        return new SuccessDataResult<List<Car>>(_carDal.GetAll(), Messages.CarsListed);
    }

    public IDataResult<Car> GetById(int id)
    {
        return new SuccessDataResult<Car>(_carDal.Get(c => c.Id == id));
    }

    public IDataResult<List<CarDetailDto>> GetCarDetails()
    {
        return new SuccessDataResult<List<CarDetailDto>>(_carDal.GetCarDetails());
    }

    [CacheAspect(duration: 10)]
    public IDataResult<List<CarDetailDto>> GetCarsByBrandId(int brandId)
    {
        return new SuccessDataResult<List<CarDetailDto>>(_carDal.GetCarDetails(c => c.BrandId == brandId));
    }

    public IDataResult<List<CarDetailDto>> GetCarsByColorId(int colorId)
    {
        return new SuccessDataResult<List<CarDetailDto>>(_carDal.GetCarDetails(c => c.ColorId == colorId), Messages.CarsListed);
    }

    [TransactionScopeAspect]
    public IResult TransactionalOperation(Car car)
    {
        _carDal.Update(car);
        _carDal.Add(car);
        return new SuccessResult(Messages.CarUpdated);
    }

    public IDataResult<CarDetailDto> GetCarDetailsById(int id)
    {
        return new SuccessDataResult<CarDetailDto>(_carDal.GetCarDetailsById(c => c.Id == id));
    }

    public IDataResult<List<CarDetailDto>> GetCarsByColorAndBrandId(int brandId, int colorId)
    {
        var cars = _carDal.GetCarDetails(b => b.BrandId == brandId && b.ColorId == colorId);
        if (cars == null)
        {
            return new ErrorDataResult<List<CarDetailDto>>(Messages.TheCarNotFound);
        }

        return new SuccessDataResult<List<CarDetailDto>>(cars);
    }

    public IDataResult<Car> AddReturnableData(Car car)
    {
        _carDal.Add(car);

        return new SuccessDataResult<Car>(car);
    }
}
