using Core.Utilities.Results;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using IResult = Core.Utilities.Results.IResult;

namespace Business.Abstract;

public interface ICarImageService
{
    IDataResult<List<CarImage>> GetAll();
    IDataResult<CarImage> GetById(int id);
    IDataResult<List<CarImage>> GetByCarId(int carId);

    IResult Add(CarImage carImage, IFormFile file, string path = null);
    IResult Update(CarImage carImage, IFormFile file, string path = null);
    IResult Delete(CarImage carImage, string path = null);
}
