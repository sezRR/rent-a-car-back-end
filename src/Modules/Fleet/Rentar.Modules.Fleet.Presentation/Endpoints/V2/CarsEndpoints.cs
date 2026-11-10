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
    private static Results<Ok<List<CarResponse>>, ProblemHttpResult> GetAll(
        ICarService carService,
        int? brandId,
        int? colorId)
    {
        var result = (brandId, colorId) switch
        {
            ({ } brand, { } color) => carService.GetCarsByColorAndBrandId(brand, color),
            ({ } brand, null) => carService.GetCarsByBrandId(brand),
            (null, { } color) => carService.GetCarsByColorId(color),
            _ => carService.GetCarDetails()
        };

        return result.Success
            ? TypedResults.Ok(result.Data.ConvertAll(CarResponse.From))
            : RestResults.Problem(result);
    }

    private static Results<Ok<CarResponse>, ProblemHttpResult> GetById(ICarService carService, int id)
    {
        var car = carService.GetCarDetailsById(id).Data;
        return car is null ? RestResults.NotFound("Car", id) : TypedResults.Ok(CarResponse.From(car));
    }

    private static Results<Created<CarResponse>, ProblemHttpResult> Create(
        ICarService carService,
        HttpRequest request,
        CarRequest body)
    {
        var car = ToCar(0, body);

        var result = carService.Add(car);
        if (!result.Success)
        {
            return RestResults.Problem(result);
        }

        var created = carService.GetCarDetailsById(car.Id).Data;
        return RestResults.Created(request, car.Id, created is null ? CarResponse.From(car) : CarResponse.From(created));
    }

    private static Results<NoContent, ProblemHttpResult> Update(ICarService carService, int id, CarRequest body)
    {
        if (carService.GetById(id).Data is null)
        {
            return RestResults.NotFound("Car", id);
        }

        var result = carService.Update(ToCar(id, body));
        return result.Success ? TypedResults.NoContent() : RestResults.Problem(result);
    }

    /// <summary>
    /// Deletes the car together with its images and their files.
    /// </summary>
    private static Results<NoContent, ProblemHttpResult> Delete(ICarService carService, int id)
    {
        var car = carService.GetById(id).Data;
        if (car is null)
        {
            return RestResults.NotFound("Car", id);
        }

        var result = carService.Delete(car);
        return result.Success ? TypedResults.NoContent() : RestResults.Problem(result);
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
