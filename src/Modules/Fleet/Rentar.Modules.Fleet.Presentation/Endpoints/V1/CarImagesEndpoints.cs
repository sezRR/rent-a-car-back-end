using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application.CarImages;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Presentation.Endpoints;
using System;
using System.Collections.Generic;
using KernelResult = Rentar.Shared.Kernel.Results.IResult;

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

    private static IResult GetAll(ICarImageService carImageService)
    {
        var result = carImageService.GetAll();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult GetById(ICarImageService carImageService, int id)
    {
        var result = carImageService.GetById(id);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult GetByCarId(ICarImageService carImageService, int carId)
    {
        var result = carImageService.GetByCarId(carId);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult Add(
        ICarImageService carImageService,
        [FromServices] TimeProvider timeProvider,
        [FromForm(Name = "carId")] string carId,
        [FromForm(Name = "file")] IFormFile file)
    {
        var carImage = new CarImage { CarId = Convert.ToInt32(carId), Date = timeProvider.GetLocalNow().DateTime };
        var result = carImageService.Add(carImage, ToUpload(file));
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult Update(
        ICarImageService carImageService,
        [FromForm(Name = "objectFile")] IFormFile objectFile,
        [FromForm(Name = "carImage")] CarImage carImage)
    {
        var result = carImageService.Update(carImage, ToUpload(objectFile));
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult Delete(ICarImageService carImageService, int id)
    {
        var carImages = carImageService.GetByCarId(id).Data;

        if (carImages.Count == 0)
        {
            return Results.BadRequest("This car do not have image!");
        }

        List<KernelResult> results = new();

        foreach (var carImage in carImages)
        {
            results.Add(carImageService.Delete(carImage));
        }

        foreach (var result in results)
        {
            if (!result.Success)
            {
                return Results.BadRequest(result);
            }
        }

        return Results.Ok(results[0]);
    }

    private static FileUpload ToUpload(IFormFile file)
    {
        return file == null ? null : new FileUpload(file.FileName, file.Length, file.OpenReadStream);
    }
}
