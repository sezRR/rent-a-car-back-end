using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application.CarImages;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Presentation.Endpoints;
using System;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Presentation.Endpoints.V1;

public class CarImagesEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("CarImages", "carimages", 1.0);

        group.MapGet("/getall", GetAll);
        group.MapGet("/getbyid", GetById);
        group.MapGet("/getbycarid", GetByCarId);
        group.MapPost("/add", Add).DisableAntiforgery();
        group.MapPost("/update", Update).DisableAntiforgery();
        group.MapDelete("/delete", Delete);
    }

    private static Ok<V1Response<List<CarImage>>> GetAll(IQueryHandler<GetCarImages, List<CarImage>> getCarImages)
    {
        return V1Results.Ok(getCarImages.Handle(new GetCarImages()), null);
    }

    private static Ok<V1Response<CarImage>> GetById(IQueryHandler<GetCarImageById, CarImage> getCarImageById, int id)
    {
        return V1Results.Ok(getCarImageById.Handle(new GetCarImageById(id)), null);
    }

    private static Ok<V1Response<List<CarImage>>> GetByCarId(IQueryHandler<GetCarImagesByCar, List<CarImage>> getCarImagesByCar, int carId)
    {
        return V1Results.Ok(getCarImagesByCar.Handle(new GetCarImagesByCar(carId)), null);
    }

    private static Ok<V1Response> Add(
        ICommandHandler<AddCarImage, CarImage> addCarImage,
        [FromForm(Name = "carId")] string carId,
        [FromForm(Name = "file")] IFormFile file)
    {
        addCarImage.Handle(new AddCarImage(Convert.ToInt32(carId), ToUpload(file)));
        return V1Results.Ok(null);
    }

    private static Ok<V1Response> Update(
        ICommandHandler<ReplaceCarImage> replaceCarImage,
        [FromForm(Name = "objectFile")] IFormFile objectFile,
        [FromForm(Name = "carImage")] CarImage carImage)
    {
        replaceCarImage.Handle(new ReplaceCarImage(carImage.Id, carImage.CarId, ToUpload(objectFile)));
        return V1Results.Ok(null);
    }

    /// <summary>
    /// Despite its name, <paramref name="id"/> is a car id: every image of that car is deleted.
    /// </summary>
    private static Results<Ok<V1Response>, BadRequest<string>> Delete(
        IQueryHandler<GetCarImagesByCar, List<CarImage>> getCarImagesByCar,
        ICommandHandler<DeleteCarImagesOfCar> deleteCarImagesOfCar,
        int id)
    {
        if (getCarImagesByCar.Handle(new GetCarImagesByCar(id)).Count == 0)
        {
            return TypedResults.BadRequest("This car do not have image!");
        }

        deleteCarImagesOfCar.Handle(new DeleteCarImagesOfCar(id));
        return V1Results.Ok(null);
    }

    private static FileUpload ToUpload(IFormFile file)
    {
        return file == null ? null : new FileUpload(file.FileName, file.Length, file.OpenReadStream);
    }
}
