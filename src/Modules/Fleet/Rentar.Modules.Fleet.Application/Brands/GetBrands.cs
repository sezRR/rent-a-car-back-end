using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Brands;

public sealed record GetBrands;

public sealed class GetBrandsHandler(IBrandDal brandDal) : IQueryHandler<GetBrands, List<Brand>>
{
    public List<Brand> Handle(GetBrands query)
    {
        return brandDal.GetAll();
    }
}
