using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WebAPI.Common;

namespace WebAPI.Endpoints.v1;

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

    private IResult GetAll(IBrandService brandService)
    {
        var result = brandService.GetAll();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult GetById(IBrandService brandService, int id)
    {
        var result = brandService.GetById(id);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Add(IBrandService brandService, Brand brand)
    {
        var result = brandService.Add(brand);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Update(IBrandService brandService, Brand brand)
    {
        var result = brandService.Update(brand);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Delete(IBrandService brandService, Brand brand)
    {
        var result = brandService.Delete(brand);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
