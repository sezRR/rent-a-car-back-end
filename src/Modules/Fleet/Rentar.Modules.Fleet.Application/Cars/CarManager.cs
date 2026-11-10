using Rentar.Modules.Fleet.Application.CarImages;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Aspects.Caching;
using Rentar.Shared.Application.Aspects.Performance;
using Rentar.Shared.Application.Aspects.Transaction;
using Rentar.Shared.Application.Aspects.Validation;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Cars;

public class CarManager(ICarDal carDal, ICarImageService carImageService) : ICarService
{
    private readonly ICarDal _carDal = carDal;
    private readonly ICarImageService _carImageService = carImageService;

    [ValidationAspect(typeof(CarValidator))]
    [CacheRemoveAspect("ICarService.Get")]
    public void Add(Car car)
    {
        _carDal.Add(car);
    }

    public void Update(Car car)
    {
        _carDal.Update(car);
    }

    [TransactionScopeAspect]
    public void Delete(Car car)
    {
        // Images are not tied to the car by a foreign key, so their rows and files are removed here.
        foreach (var carImage in _carImageService.GetByCarId(car.Id))
        {
            _carImageService.Delete(carImage);
        }

        _carDal.Delete(car);
    }

    [PerformanceAspect(5)]
    public List<Car> GetAll()
    {
        return _carDal.GetAll();
    }

    public Car GetById(int id)
    {
        return _carDal.Get(c => c.Id == id);
    }

    public List<CarDetailDto> GetCarDetails()
    {
        return _carDal.GetCarDetails();
    }

    [CacheAspect(duration: 10)]
    public List<CarDetailDto> GetCarsByBrandId(int brandId)
    {
        return _carDal.GetCarDetails(c => c.BrandId == brandId);
    }

    public List<CarDetailDto> GetCarsByColorId(int colorId)
    {
        return _carDal.GetCarDetails(c => c.ColorId == colorId);
    }

    [TransactionScopeAspect]
    public void TransactionalOperation(Car car)
    {
        _carDal.Update(car);
        _carDal.Add(car);
    }

    public CarDetailDto GetCarDetailsById(int id)
    {
        return _carDal.GetCarDetailsById(c => c.Id == id);
    }

    public List<CarDetailDto> GetCarsByColorAndBrandId(int brandId, int colorId)
    {
        return _carDal.GetCarDetails(b => b.BrandId == brandId && b.ColorId == colorId);
    }

    public Car AddReturnableData(Car car)
    {
        _carDal.Add(car);
        return car;
    }
}
