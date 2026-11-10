#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application.Cars;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Presentation.Endpoints;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Presentation.Endpoints.V2;

public class CarsEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Cars", "cars", 2.0);

        group.MapGet("/", GetAll);
        group.MapGet("/{id:int}", GetById)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPost("/", Create)
            .ProducesValidationProblem();
        group.MapPut("/{id:int}", Update)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapDelete("/{id:int}", Delete)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Lists cars, optionally only those of a brand, a color, or both. Each filter is its own use case.
    /// </summary>
    private static Ok<List<CarResponse>> GetAll(
        IQueryHandler<GetCarDetails, List<CarDetailDto>> getCarDetails,
        IQueryHandler<GetCarsByBrand, List<CarDetailDto>> getCarsByBrand,
        IQueryHandler<GetCarsByColor, List<CarDetailDto>> getCarsByColor,
        IQueryHandler<GetCarsByBrandAndColor, List<CarDetailDto>> getCarsByBrandAndColor,
        int? brandId,
        int? colorId)
    {
        var cars = (brandId, colorId) switch
        {
            ({ } brand, { } color) => getCarsByBrandAndColor.Handle(new GetCarsByBrandAndColor(brand, color)),
            ({ } brand, null) => getCarsByBrand.Handle(new GetCarsByBrand(brand)),
            (null, { } color) => getCarsByColor.Handle(new GetCarsByColor(color)),
            _ => getCarDetails.Handle(new GetCarDetails())
        };

        return TypedResults.Ok(cars.ConvertAll(CarResponse.From));
    }

    private static Results<Ok<CarResponse>, ProblemHttpResult> GetById(
        IQueryHandler<GetCarDetailsById, CarDetailDto> getCarDetailsById,
        int id)
    {
        var car = getCarDetailsById.Handle(new GetCarDetailsById(id));
        return car is null ? RestResults.NotFound("Car", id) : TypedResults.Ok(CarResponse.From(car));
    }

    private static Created<CarResponse> Create(
        ICommandHandler<CreateCar, Car> createCar,
        IQueryHandler<GetCarDetailsById, CarDetailDto> getCarDetailsById,
        HttpRequest request,
        CarRequest body)
    {
        var car = createCar.Handle(new CreateCar(
            body.BrandId, body.ColorId, body.ModelYear, body.DailyPrice, body.Description, body.MinimumFindeksRating));

        var created = getCarDetailsById.Handle(new GetCarDetailsById(car.Id));
        return RestResults.Created(request, car.Id, created is null ? CarResponse.From(car) : CarResponse.From(created));
    }

    private static Results<NoContent, ProblemHttpResult> Update(
        IQueryHandler<GetCarById, Car> getCarById,
        ICommandHandler<UpdateCar> updateCar,
        int id,
        CarRequest body)
    {
        if (getCarById.Handle(new GetCarById(id)) is null)
        {
            return RestResults.NotFound("Car", id);
        }

        updateCar.Handle(new UpdateCar(
            id, body.BrandId, body.ColorId, body.ModelYear, body.DailyPrice, body.Description, body.MinimumFindeksRating));
        return TypedResults.NoContent();
    }

    /// <summary>
    /// Deletes the car together with its images and their files.
    /// </summary>
    private static Results<NoContent, ProblemHttpResult> Delete(
        IQueryHandler<GetCarById, Car> getCarById,
        ICommandHandler<DeleteCar> deleteCar,
        int id)
    {
        if (getCarById.Handle(new GetCarById(id)) is null)
        {
            return RestResults.NotFound("Car", id);
        }

        deleteCar.Handle(new DeleteCar(id));
        return TypedResults.NoContent();
    }
}
