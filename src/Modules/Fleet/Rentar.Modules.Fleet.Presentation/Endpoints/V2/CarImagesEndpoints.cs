#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application.CarImages;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Presentation.Endpoints;
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

    private static Ok<List<CarImage>> GetAll(IQueryHandler<GetCarImages, List<CarImage>> getCarImages)
    {
        return TypedResults.Ok(getCarImages.Handle(new GetCarImages()));
    }

    private static Ok<List<CarImage>> GetByCar(IQueryHandler<GetCarImagesByCar, List<CarImage>> getCarImagesByCar, int carId)
    {
        return TypedResults.Ok(getCarImagesByCar.Handle(new GetCarImagesByCar(carId)));
    }

    private static Results<Ok<CarImage>, ProblemHttpResult> GetById(
        IQueryHandler<GetCarImageById, CarImage> getCarImageById,
        int carId,
        int imageId)
    {
        var carImage = Find(getCarImageById, carId, imageId);
        return carImage is null ? RestResults.NotFound("Car image", imageId) : TypedResults.Ok(carImage);
    }

    /// <summary>
    /// Stores a PNG or JPEG sent as the <c>file</c> form field; without one the car gets the default image.
    /// </summary>
    private static Created<CarImage> Upload(
        ICommandHandler<AddCarImage, CarImage> addCarImage,
        HttpRequest request,
        int carId,
        IFormFile? file)
    {
        var carImage = addCarImage.Handle(new AddCarImage(carId, ToUpload(file)));
        return RestResults.Created(request, carImage.Id, carImage);
    }

    /// <summary>
    /// Replaces the image's file with the PNG or JPEG sent as the <c>file</c> form field.
    /// </summary>
    private static Results<NoContent, ProblemHttpResult> Replace(
        IQueryHandler<GetCarImageById, CarImage> getCarImageById,
        ICommandHandler<ReplaceCarImage> replaceCarImage,
        int carId,
        int imageId,
        IFormFile? file)
    {
        if (Find(getCarImageById, carId, imageId) is null)
        {
            return RestResults.NotFound("Car image", imageId);
        }

        // A missing file is turned down by the use case with the same message as an unsupported one.
        replaceCarImage.Handle(new ReplaceCarImage(imageId, carId, ToUpload(file)));
        return TypedResults.NoContent();
    }

    private static Results<NoContent, ProblemHttpResult> Delete(
        IQueryHandler<GetCarImageById, CarImage> getCarImageById,
        ICommandHandler<DeleteCarImage> deleteCarImage,
        int carId,
        int imageId)
    {
        if (Find(getCarImageById, carId, imageId) is null)
        {
            return RestResults.NotFound("Car image", imageId);
        }

        deleteCarImage.Handle(new DeleteCarImage(imageId));
        return TypedResults.NoContent();
    }

    /// <summary>
    /// Deletes every image of the car. A car without images is already in that state, so this also answers 204.
    /// </summary>
    private static NoContent DeleteAll(ICommandHandler<DeleteCarImagesOfCar> deleteCarImagesOfCar, int carId)
    {
        deleteCarImagesOfCar.Handle(new DeleteCarImagesOfCar(carId));
        return TypedResults.NoContent();
    }

    /// <summary>
    /// An image is only found under the car it belongs to.
    /// </summary>
    private static CarImage? Find(IQueryHandler<GetCarImageById, CarImage> getCarImageById, int carId, int imageId)
    {
        var carImage = getCarImageById.Handle(new GetCarImageById(imageId));
        return carImage?.CarId == carId ? carImage : null;
    }

    private static FileUpload? ToUpload(IFormFile? file)
    {
        return file is null ? null : new FileUpload(file.FileName, file.Length, file.OpenReadStream);
    }
}
