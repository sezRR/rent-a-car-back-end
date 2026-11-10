using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Kernel.Results;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Brands;

public class BrandManager(IBrandDal brandDal) : IBrandService
{
    private readonly IBrandDal _brandDal = brandDal;

    [ValidationAspect(typeof(BrandValidator))]
    public IResult Add(Brand brand)
    {
        _brandDal.Add(brand);

        return new SuccessResult(Messages.BrandAdded);
    }

    public IResult Delete(Brand brand)
    {
        _brandDal.Delete(brand);

        return new SuccessResult(Messages.BrandDeleted);
    }

    public IDataResult<List<Brand>> GetAll()
    {
        return new SuccessDataResult<List<Brand>>(_brandDal.GetAll(), Messages.BrandsListed);
    }

    public IDataResult<Brand> GetById(int brandId)
    {
        return new SuccessDataResult<Brand>(_brandDal.Get(b => b.Id == brandId));
    }

    public IResult Update(Brand brand)
    {
        _brandDal.Update(brand);

        return new SuccessResult(Messages.BrandUpdated);
    }
}
