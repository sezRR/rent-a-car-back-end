#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application.CarImages;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Presentation.Endpoints;
using System;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Presentation.Endpoints.V2;

/// <summary>
/// Images belong to a car, so they are addressed under it: <c>/cars/{carId}/images</c>.
/// Only listing every image of every car has its own top-level collection, <c>/car-images</c>.
/// </summary>
public class CarImagesEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var images = builder.MapVersionedGroup("CarImages", "cars/{carId:int}/images", 2.0);

        images.MapGet("/", GetByCar);
        images.MapGet("/{imageId:int}", GetById)
            .ProducesProblem(StatusCodes.Status404NotFound);
        images.MapPost("/", Upload)
            .DisableAntiforgery()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        images.MapPut("/{imageId:int}", Replace)
            .DisableAntiforgery()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        images.MapDelete("/{imageId:int}", Delete)
            .ProducesProblem(StatusCodes.Status404NotFound);
        images.MapDelete("/", DeleteAll);

        builder.MapVersionedGroup("CarImages", "car-images", 2.0)
            .MapGet("/", GetAll);
    }

    private static Results<Ok<List<CarImage>>, ProblemHttpResult> GetAll(ICarImageService carImageService)
    {
        var result = carImageService.GetAll();
        return result.Success ? TypedResults.Ok(result.Data) : RestResults.Problem(result);
    }

    private static Results<Ok<List<CarImage>>, ProblemHttpResult> GetByCar(ICarImageService carImageService, int carId)
    {
        var result = carImageService.GetByCarId(carId);
        return result.Success ? TypedResults.Ok(result.Data) : RestResults.Problem(result);
    }

    private static Results<Ok<CarImage>, ProblemHttpResult> GetById(ICarImageService carImageService, int carId, int imageId)
    {
        var carImage = Find(carImageService, carId, imageId);
        return carImage is null ? RestResults.NotFound("Car image", imageId) : TypedResults.Ok(carImage);
    }

    /// <summary>
    /// Stores a PNG or JPEG sent as the <c>file</c> form field; without one the car gets the default image.
    /// </summary>
    private static Results<Created<CarImage>, ProblemHttpResult> Upload(
        ICarImageService carImageService,
        [FromServices] TimeProvider timeProvider,
        HttpRequest request,
        int carId,
        IFormFile? file)
    {
        var carImage = new CarImage { CarId = carId, Date = timeProvider.GetLocalNow().DateTime };

        var result = carImageService.Add(carImage, ToUpload(file));
        return result.Success ? RestResults.Created(request, carImage.Id, carImage) : RestResults.Problem(result);
    }

    /// <summary>
    /// Replaces the image's file with the PNG or JPEG sent as the <c>file</c> form field.
    /// </summary>
    private static Results<NoContent, ProblemHttpResult> Replace(
        ICarImageService carImageService,
        int carId,
        int imageId,
        IFormFile? file)
    {
        if (Find(carImageService, carId, imageId) is null)
        {
            return RestResults.NotFound("Car image", imageId);
        }

        var result = carImageService.Update(new CarImage { Id = imageId, CarId = carId }, ToUpload(file));
        return result.Success ? TypedResults.NoContent() : RestResults.Problem(result);
    }

    private static Results<NoContent, ProblemHttpResult> Delete(ICarImageService carImageService, int carId, int imageId)
    {
        var carImage = Find(carImageService, carId, imageId);
        if (carImage is null)
        {
            return RestResults.NotFound("Car image", imageId);
        }

        var result = carImageService.Delete(carImage);
        return result.Success ? TypedResults.NoContent() : RestResults.Problem(result);
    }

    /// <summary>
    /// Deletes every image of the car. A car without images is already in that state, so this also answers 204.
    /// </summary>
    private static Results<NoContent, ProblemHttpResult> DeleteAll(ICarImageService carImageService, int carId)
    {
        foreach (var carImage in carImageService.GetByCarId(carId).Data)
        {
            var result = carImageService.Delete(carImage);
            if (!result.Success)
            {
                return RestResults.Problem(result);
            }
        }

        return TypedResults.NoContent();
    }

    /// <summary>
    /// An image is only found under the car it belongs to.
    /// </summary>
    private static CarImage? Find(ICarImageService carImageService, int carId, int imageId)
    {
        var carImage = carImageService.GetById(imageId).Data;
        return carImage?.CarId == carId ? carImage : null;
    }

    private static FileUpload? ToUpload(IFormFile? file)
    {
        return file is null ? null : new FileUpload(file.FileName, file.Length, file.OpenReadStream);
    }
}
