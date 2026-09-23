using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WebAPI.Common;

namespace WebAPI.Endpoints.v1;

public class ColorsEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Colors", "colors", 1.0);

        group.MapGet("/getall", GetAll);
        group.MapGet("/getbyid", GetById);
        group.MapPost("/add", Add);
        group.MapPost("/update", Update);
        group.MapPost("/delete", Delete);
    }

    private IResult GetAll(IColorService colorService)
    {
        var result = colorService.GetAll();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult GetById(IColorService colorService, int id)
    {
        var result = colorService.GetById(id);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Add(IColorService colorService, Color color)
    {
        var result = colorService.Add(color);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Update(IColorService colorService, Color color)
    {
        var result = colorService.Update(color);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Delete(IColorService colorService, Color color)
    {
        var result = colorService.Delete(color);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
