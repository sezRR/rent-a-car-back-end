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

    private static Ok<V1Response<List<CarImage>>> GetAll(ICarImageService carImageService)
    {
        return V1Results.Ok(carImageService.GetAll(), null);
    }

    private static Ok<V1Response<CarImage>> GetById(ICarImageService carImageService, int id)
    {
        return V1Results.Ok(carImageService.GetById(id), null);
    }

    private static Ok<V1Response<List<CarImage>>> GetByCarId(ICarImageService carImageService, int carId)
    {
        return V1Results.Ok(carImageService.GetByCarId(carId), null);
    }

    private static Ok<V1Response> Add(
        ICarImageService carImageService,
        [FromServices] TimeProvider timeProvider,
        [FromForm(Name = "carId")] string carId,
        [FromForm(Name = "file")] IFormFile file)
    {
        var carImage = new CarImage { CarId = Convert.ToInt32(carId), Date = timeProvider.GetLocalNow().DateTime };
        carImageService.Add(carImage, ToUpload(file));
        return V1Results.Ok(null);
    }

    private static Ok<V1Response> Update(
        ICarImageService carImageService,
        [FromForm(Name = "objectFile")] IFormFile objectFile,
        [FromForm(Name = "carImage")] CarImage carImage)
    {
        carImageService.Update(carImage, ToUpload(objectFile));
        return V1Results.Ok(null);
    }

    /// <summary>
    /// Despite its name, <paramref name="id"/> is a car id: every image of that car is deleted.
    /// </summary>
    private static Results<Ok<V1Response>, BadRequest<string>> Delete(ICarImageService carImageService, int id)
    {
        var carImages = carImageService.GetByCarId(id);

        if (carImages.Count == 0)
        {
            return TypedResults.BadRequest("This car do not have image!");
        }

        foreach (var carImage in carImages)
        {
            carImageService.Delete(carImage);
        }

        return V1Results.Ok(null);
    }

    private static FileUpload ToUpload(IFormFile file)
    {
        return file == null ? null : new FileUpload(file.FileName, file.Length, file.OpenReadStream);
    }
}
