#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application.Colors;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Presentation.Endpoints;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Presentation.Endpoints.V2;

public class ColorsEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Colors", "colors", 2.0);

        group.MapGet("/", GetAll);
        group.MapGet("/{id:int}", GetById)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPost("/", Create)
            .ProducesValidationProblem();
        group.MapPut("/{id:int}", Update)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapDelete("/{id:int}", Delete)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static Ok<List<Color>> GetAll(IColorService colorService)
    {
        return TypedResults.Ok(colorService.GetAll());
    }

    private static Results<Ok<Color>, ProblemHttpResult> GetById(IColorService colorService, int id)
    {
        var color = colorService.GetById(id);
        return color is null ? RestResults.NotFound("Color", id) : TypedResults.Ok(color);
    }

    private static Created<Color> Create(IColorService colorService, HttpRequest request, ColorRequest body)
    {
        var color = new Color { ColorName = body.ColorName };

        colorService.Add(color);
        return RestResults.Created(request, color.Id, color);
    }

    private static Results<NoContent, ProblemHttpResult> Update(IColorService colorService, int id, ColorRequest body)
    {
        if (colorService.GetById(id) is null)
        {
            return RestResults.NotFound("Color", id);
        }

        colorService.Update(new Color { Id = id, ColorName = body.ColorName });
        return TypedResults.NoContent();
    }

    private static Results<NoContent, ProblemHttpResult> Delete(IColorService colorService, int id)
    {
        var color = colorService.GetById(id);
        if (color is null)
        {
            return RestResults.NotFound("Color", id);
        }

        colorService.Delete(color);
        return TypedResults.NoContent();
    }
}
