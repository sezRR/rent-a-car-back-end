using Rentar.Modules.Fleet.Domain;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Cars;

public interface ICarService
{
    List<Car> GetAll();

    /// <returns>The car, or <c>null</c> when there is none with that id.</returns>
    Car GetById(int id);

    List<CarDetailDto> GetCarsByBrandId(int brandId);
    List<CarDetailDto> GetCarsByColorId(int colorId);
    List<CarDetailDto> GetCarsByColorAndBrandId(int brandId, int colorId);

    /// <summary>Stores the car and sets its <see cref="Car.Id"/>.</summary>
    void Add(Car car);

    /// <summary>Stores the car, bypassing validation, and returns it with its id set.</summary>
    Car AddReturnableData(Car car);

    void Update(Car car);

    /// <summary>Deletes the car together with its images and their files.</summary>
    void Delete(Car car);

    List<CarDetailDto> GetCarDetails();

    /// <returns>The car's details, or <c>null</c> when there is no such car or its brand or color is not stored.</returns>
    CarDetailDto GetCarDetailsById(int id);

    void TransactionalOperation(Car car);
}
