using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Rentar.Modules.Fleet.Application.Cars;

public interface ICarDal : IEntityRepository<Car>
{
    List<CarDetailDto> GetCarDetails(Expression<Func<Car, bool>> filter = null);
    CarDetailDto GetCarDetailsById(Expression<Func<Car, bool>> filter);
}
