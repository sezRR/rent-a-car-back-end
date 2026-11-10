using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application;
using Rentar.Modules.Fleet.Application.Cars;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Presentation.Endpoints;
using System.Collections.Generic;

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

    private static Ok<V1Response<List<Car>>> GetAll(ICarService carService)
    {
        return V1Results.Ok(carService.GetAll(), Messages.CarsListed);
    }

    private static Ok<V1Response<List<CarDetailDto>>> GetCarDetails(ICarService carService)
    {
        return V1Results.Ok(carService.GetCarDetails(), null);
    }

    private static Ok<V1Response<CarDetailDto>> GetCarDetailsById(ICarService carService, int id)
    {
        return V1Results.Ok(carService.GetCarDetailsById(id), null);
    }

    private static Ok<V1Response<List<CarDetailDto>>> GetByBrandId(ICarService carService, int brandId)
    {
        return V1Results.Ok(carService.GetCarsByBrandId(brandId), null);
    }

    private static Ok<V1Response<List<CarDetailDto>>> GetByColorId(ICarService carService, int colorId)
    {
        return V1Results.Ok(carService.GetCarsByColorId(colorId), Messages.CarsListed);
    }

    private static Ok<V1Response<List<CarDetailDto>>> GetCarsByBrandIdAndColorId(ICarService carService, int brandId, int colorId)
    {
        return V1Results.Ok(carService.GetCarsByColorAndBrandId(brandId, colorId), null);
    }

    private static Ok<V1Response> Add(ICarService carService, Car car)
    {
        carService.Add(car);
        return V1Results.Ok(Messages.CarAdded);
    }

    private static Ok<V1Response<Car>> AddReturnableData(ICarService carService, Car car)
    {
        return V1Results.Ok(carService.AddReturnableData(car), null);
    }

    private static Ok<V1Response> Update(ICarService carService, Car car)
    {
        carService.Update(car);
        return V1Results.Ok(Messages.CarUpdated);
    }

    private static Ok<V1Response> Delete(ICarService carService, Car car)
    {
        carService.Delete(car);
        return V1Results.Ok(Messages.CarDeleted);
    }
}
