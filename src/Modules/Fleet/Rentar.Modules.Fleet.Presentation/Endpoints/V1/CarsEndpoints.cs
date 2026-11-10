using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application;
using Rentar.Modules.Fleet.Application.Cars;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;
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

    private static Ok<V1Response<List<Car>>> GetAll(IQueryHandler<GetCars, List<Car>> getCars)
    {
        return V1Results.Ok(getCars.Handle(new GetCars()), Messages.CarsListed);
    }

    private static Ok<V1Response<List<CarDetailDto>>> GetCarDetails(IQueryHandler<GetCarDetails, List<CarDetailDto>> getCarDetails)
    {
        return V1Results.Ok(getCarDetails.Handle(new GetCarDetails()), null);
    }

    private static Ok<V1Response<CarDetailDto>> GetCarDetailsById(IQueryHandler<GetCarDetailsById, CarDetailDto> getCarDetailsById, int id)
    {
        return V1Results.Ok(getCarDetailsById.Handle(new GetCarDetailsById(id)), null);
    }

    private static Ok<V1Response<List<CarDetailDto>>> GetByBrandId(IQueryHandler<GetCarsByBrand, List<CarDetailDto>> getCarsByBrand, int brandId)
    {
        return V1Results.Ok(getCarsByBrand.Handle(new GetCarsByBrand(brandId)), null);
    }

    private static Ok<V1Response<List<CarDetailDto>>> GetByColorId(IQueryHandler<GetCarsByColor, List<CarDetailDto>> getCarsByColor, int colorId)
    {
        return V1Results.Ok(getCarsByColor.Handle(new GetCarsByColor(colorId)), Messages.CarsListed);
    }

    private static Ok<V1Response<List<CarDetailDto>>> GetCarsByBrandIdAndColorId(
        IQueryHandler<GetCarsByBrandAndColor, List<CarDetailDto>> getCarsByBrandAndColor,
        int brandId,
        int colorId)
    {
        return V1Results.Ok(getCarsByBrandAndColor.Handle(new GetCarsByBrandAndColor(brandId, colorId)), null);
    }

    private static Ok<V1Response> Add(ICommandHandler<CreateCar, Car> createCar, CreateCar command)
    {
        createCar.Handle(command);
        return V1Results.Ok(Messages.CarAdded);
    }

    /// <summary>
    /// Like <c>add</c>, but answers the stored car.
    /// </summary>
    private static Ok<V1Response<Car>> AddReturnableData(ICommandHandler<CreateCar, Car> createCar, CreateCar command)
    {
        return V1Results.Ok(createCar.Handle(command), null);
    }

    private static Ok<V1Response> Update(ICommandHandler<UpdateCar> updateCar, UpdateCar command)
    {
        updateCar.Handle(command);
        return V1Results.Ok(Messages.CarUpdated);
    }

    private static Ok<V1Response> Delete(ICommandHandler<DeleteCar> deleteCar, DeleteCar command)
    {
        deleteCar.Handle(command);
        return V1Results.Ok(Messages.CarDeleted);
    }
}
