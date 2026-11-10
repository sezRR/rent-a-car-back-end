using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Brands;

public class BrandManager(IBrandDal brandDal) : IBrandService
{
    private readonly IBrandDal _brandDal = brandDal;

    [ValidationAspect(typeof(BrandValidator))]
    public void Add(Brand brand)
    {
        _brandDal.Add(brand);
    }

    public void Delete(Brand brand)
    {
        _brandDal.Delete(brand);
    }

    public List<Brand> GetAll()
    {
        return _brandDal.GetAll();
    }

    public Brand GetById(int brandId)
    {
        return _brandDal.Get(b => b.Id == brandId);
    }

    public void Update(Brand brand)
    {
        _brandDal.Update(brand);
    }
}
