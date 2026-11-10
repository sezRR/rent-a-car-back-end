using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Kernel.Results;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.CarImages;

public interface ICarImageService
{
    IDataResult<List<CarImage>> GetAll();
    IDataResult<CarImage> GetById(int id);
    IDataResult<List<CarImage>> GetByCarId(int carId);

    IResult Add(CarImage carImage, FileUpload file);
    IResult Update(CarImage carImage, FileUpload file);
    IResult Delete(CarImage carImage);
}
