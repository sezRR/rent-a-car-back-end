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

    private static Results<Ok<List<Color>>, ProblemHttpResult> GetAll(IColorService colorService)
    {
        var result = colorService.GetAll();
        return result.Success ? TypedResults.Ok(result.Data) : RestResults.Problem(result);
    }

    private static Results<Ok<Color>, ProblemHttpResult> GetById(IColorService colorService, int id)
    {
        var color = colorService.GetById(id).Data;
        return color is null ? RestResults.NotFound("Color", id) : TypedResults.Ok(color);
    }

    private static Results<Created<Color>, ProblemHttpResult> Create(
        IColorService colorService,
        HttpRequest request,
        ColorRequest body)
    {
        var color = new Color { ColorName = body.ColorName };

        var result = colorService.Add(color);
        return result.Success ? RestResults.Created(request, color.Id, color) : RestResults.Problem(result);
    }

    private static Results<NoContent, ProblemHttpResult> Update(IColorService colorService, int id, ColorRequest body)
    {
        if (colorService.GetById(id).Data is null)
        {
            return RestResults.NotFound("Color", id);
        }

        var result = colorService.Update(new Color { Id = id, ColorName = body.ColorName });
        return result.Success ? TypedResults.NoContent() : RestResults.Problem(result);
    }

    private static Results<NoContent, ProblemHttpResult> Delete(IColorService colorService, int id)
    {
        var color = colorService.GetById(id).Data;
        if (color is null)
        {
            return RestResults.NotFound("Color", id);
        }

        var result = colorService.Delete(color);
        return result.Success ? TypedResults.NoContent() : RestResults.Problem(result);
    }
}
