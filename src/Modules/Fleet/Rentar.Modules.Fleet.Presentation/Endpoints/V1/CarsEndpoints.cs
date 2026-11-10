using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application.Cars;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Presentation.Endpoints;

namespace Rentar.Modules.Fleet.Presentation.Endpoints.V1;

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

    private static IResult GetAll(ICarService carService)
    {
        var result = carService.GetAll();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult GetCarDetails(ICarService carService)
    {
        var result = carService.GetCarDetails();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult GetCarDetailsById(ICarService carService, int id)
    {
        var result = carService.GetCarDetailsById(id);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult GetByBrandId(ICarService carService, int brandId)
    {
        var result = carService.GetCarsByBrandId(brandId);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult GetByColorId(ICarService carService, int colorId)
    {
        var result = carService.GetCarsByColorId(colorId);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult GetCarsByBrandIdAndColorId(ICarService carService, int brandId, int colorId)
    {
        var result = carService.GetCarsByColorAndBrandId(brandId, colorId);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult Add(ICarService carService, Car car)
    {
        var result = carService.Add(car);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult AddReturnableData(ICarService carService, Car car)
    {
        var result = carService.AddReturnableData(car);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult Update(ICarService carService, Car car)
    {
        var result = carService.Update(car);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult Delete(ICarService carService, Car car)
    {
        var result = carService.Delete(car);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
