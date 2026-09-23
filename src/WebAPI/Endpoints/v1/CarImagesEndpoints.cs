using System;
using System.Collections.Generic;
using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WebAPI.Common;
using CoreResult = Core.Utilities.Results.IResult;

namespace WebAPI.Endpoints.v1;

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

    private IResult GetAll(ICarImageService carImageService)
    {
        var result = carImageService.GetAll();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult GetById(ICarImageService carImageService, int id)
    {
        var result = carImageService.GetById(id);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult GetByCarId(ICarImageService carImageService, int carId)
    {
        var result = carImageService.GetByCarId(carId);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Add(
        ICarImageService carImageService,
        [FromForm(Name = "carId")] string carId,
        [FromForm(Name = "file")] IFormFile file)
    {
        var result = carImageService.Add(new CarImage { CarId = Convert.ToInt32(carId), Date = DateTime.Now }, file);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Update(
        ICarImageService carImageService,
        [FromForm(Name = "objectFile")] IFormFile objectFile,
        [FromForm(Name = "carImage")] CarImage carImage)
    {
        var result = carImageService.Update(carImage, objectFile);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Delete(ICarImageService carImageService, int id)
    {
        var carImages = carImageService.GetByCarId(id).Data;

        if (carImages.Count == 0)
        {
            return Results.BadRequest("This car do not have image!");
        }

        List<CoreResult> results = new();

        foreach (var carImage in carImages)
        {
            var result = carImageService.Delete(carImage);
            results.Add(result);
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
}
