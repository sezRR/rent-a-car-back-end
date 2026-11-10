using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application;
using Rentar.Modules.Fleet.Application.Brands;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Presentation.Endpoints;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Presentation.Endpoints.V1;

public class BrandsEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Brands", "brands", 1.0);

        group.MapGet("/getall", GetAll);
        group.MapGet("/getbyid", GetById);
        group.MapPost("/add", Add);
        group.MapPost("/update", Update);
        group.MapPost("/delete", Delete);
    }

    private static Ok<V1Response<List<Brand>>> GetAll(IBrandService brandService)
    {
        return V1Results.Ok(brandService.GetAll(), Messages.BrandsListed);
    }

    private static Ok<V1Response<Brand>> GetById(IBrandService brandService, int id)
    {
        return V1Results.Ok(brandService.GetById(id), null);
    }

    private static Ok<V1Response> Add(IBrandService brandService, Brand brand)
    {
        brandService.Add(brand);
        return V1Results.Ok(Messages.BrandAdded);
    }

    private static Ok<V1Response> Update(IBrandService brandService, Brand brand)
    {
        brandService.Update(brand);
        return V1Results.Ok(Messages.BrandUpdated);
    }

    private static Ok<V1Response> Delete(IBrandService brandService, Brand brand)
    {
        brandService.Delete(brand);
        return V1Results.Ok(Messages.BrandDeleted);
    }
}
