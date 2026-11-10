using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Kernel.Results;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Brands;

public interface IBrandService
{
    IDataResult<List<Brand>> GetAll();
    IDataResult<Brand> GetById(int brandId);

    IResult Add(Brand brand);
    IResult Update(Brand brand);
    IResult Delete(Brand brand);
}
