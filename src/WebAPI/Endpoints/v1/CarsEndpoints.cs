using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WebAPI.Common;

namespace WebAPI.Endpoints.v1;

public class CarsEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Cars", "cars", 1.0);

        group.MapGet("/getall", GetAll);
        group.MapGet("/getcardetails", GetCarDetails);
        group.MapGet("/getcardetailsbyid", GetCarDetailsById);
        group.MapGet("/getcarsbybrandid", GetByBrandId);
        group.MapGet("/getcarsbycolorid", GetByColorId);
        group.MapGet("/getcarsbybrandidandcolorid", GetCarsByBrandIdAndColorId);
        group.MapPost("/add", Add);
        group.MapPost("/addreturnabledata", AddReturnableData);
        group.MapPost("/update", Update);
        group.MapPost("/delete", Delete);
    }

    private IResult GetAll(ICarService carService)
    {
        var result = carService.GetAll();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult GetCarDetails(ICarService carService)
    {
        var result = carService.GetCarDetails();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult GetCarDetailsById(ICarService carService, int id)
    {
        var result = carService.GetCarDetailsById(id);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult GetByBrandId(ICarService carService, int brandId)
    {
        var result = carService.GetCarsByBrandId(brandId);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult GetByColorId(ICarService carService, int colorId)
    {
        var result = carService.GetCarsByColorId(colorId);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult GetCarsByBrandIdAndColorId(ICarService carService, int brandId, int colorId)
    {
        var result = carService.GetCarsByColorAndBrandId(brandId, colorId);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Add(ICarService carService, Car car)
    {
        var result = carService.Add(car);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult AddReturnableData(ICarService carService, Car car)
    {
        var result = carService.AddReturnableData(car);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Update(ICarService carService, Car car)
    {
        var result = carService.Update(car);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Delete(ICarService carService, Car car)
    {
        var result = carService.Delete(car);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
