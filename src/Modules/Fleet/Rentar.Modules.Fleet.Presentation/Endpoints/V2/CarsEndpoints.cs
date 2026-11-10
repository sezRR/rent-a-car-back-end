#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application.Cars;
using Rentar.Modules.Fleet.Domain;
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
    /// Lists cars, optionally only those of a brand, a color, or both.
    /// </summary>
    private static Ok<List<CarResponse>> GetAll(ICarService carService, int? brandId, int? colorId)
    {
        var cars = (brandId, colorId) switch
        {
            ({ } brand, { } color) => carService.GetCarsByColorAndBrandId(brand, color),
            ({ } brand, null) => carService.GetCarsByBrandId(brand),
            (null, { } color) => carService.GetCarsByColorId(color),
            _ => carService.GetCarDetails()
        };

        return TypedResults.Ok(cars.ConvertAll(CarResponse.From));
    }

    private static Results<Ok<CarResponse>, ProblemHttpResult> GetById(ICarService carService, int id)
    {
        var car = carService.GetCarDetailsById(id);
        return car is null ? RestResults.NotFound("Car", id) : TypedResults.Ok(CarResponse.From(car));
    }

    private static Created<CarResponse> Create(ICarService carService, HttpRequest request, CarRequest body)
    {
        var car = ToCar(0, body);
        carService.Add(car);

        var created = carService.GetCarDetailsById(car.Id);
        return RestResults.Created(request, car.Id, created is null ? CarResponse.From(car) : CarResponse.From(created));
    }

    private static Results<NoContent, ProblemHttpResult> Update(ICarService carService, int id, CarRequest body)
    {
        if (carService.GetById(id) is null)
        {
            return RestResults.NotFound("Car", id);
        }

        carService.Update(ToCar(id, body));
        return TypedResults.NoContent();
    }

    /// <summary>
    /// Deletes the car together with its images and their files.
    /// </summary>
    private static Results<NoContent, ProblemHttpResult> Delete(ICarService carService, int id)
    {
        var car = carService.GetById(id);
        if (car is null)
        {
            return RestResults.NotFound("Car", id);
        }

        carService.Delete(car);
        return TypedResults.NoContent();
    }

    private static Car ToCar(int id, CarRequest body)
    {
        return new Car
        {
            Id = id,
            BrandId = body.BrandId,
            ColorId = body.ColorId,
            ModelYear = body.ModelYear,
            DailyPrice = body.DailyPrice,
            Description = body.Description,
            MinimumFindeksRating = body.MinimumFindeksRating
        };
    }
}
