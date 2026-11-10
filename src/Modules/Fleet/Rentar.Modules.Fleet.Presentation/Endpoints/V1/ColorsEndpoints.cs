using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application.Colors;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Presentation.Endpoints;

namespace Rentar.Modules.Fleet.Presentation.Endpoints.V1;

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

    private static IResult GetAll(IColorService colorService)
    {
        var result = colorService.GetAll();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult GetById(IColorService colorService, int id)
    {
        var result = colorService.GetById(id);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult Add(IColorService colorService, Color color)
    {
        var result = colorService.Add(color);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult Update(IColorService colorService, Color color)
    {
        var result = colorService.Update(color);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult Delete(IColorService colorService, Color color)
    {
        var result = colorService.Delete(color);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
