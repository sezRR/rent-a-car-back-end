#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application.Colors;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;
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

    private static Ok<List<Color>> GetAll(IQueryHandler<GetColors, List<Color>> getColors)
    {
        return TypedResults.Ok(getColors.Handle(new GetColors()));
    }

    private static Results<Ok<Color>, ProblemHttpResult> GetById(IQueryHandler<GetColorById, Color> getColorById, int id)
    {
        var color = getColorById.Handle(new GetColorById(id));
        return color is null ? RestResults.NotFound("Color", id) : TypedResults.Ok(color);
    }

    private static Created<Color> Create(ICommandHandler<CreateColor, Color> createColor, HttpRequest request, ColorRequest body)
    {
        var color = createColor.Handle(new CreateColor(body.ColorName));
        return RestResults.Created(request, color.Id, color);
    }

    private static Results<NoContent, ProblemHttpResult> Update(
        IQueryHandler<GetColorById, Color> getColorById,
        ICommandHandler<UpdateColor> updateColor,
        int id,
        ColorRequest body)
    {
        if (getColorById.Handle(new GetColorById(id)) is null)
        {
            return RestResults.NotFound("Color", id);
        }

        updateColor.Handle(new UpdateColor(id, body.ColorName));
        return TypedResults.NoContent();
    }

    private static Results<NoContent, ProblemHttpResult> Delete(
        IQueryHandler<GetColorById, Color> getColorById,
        ICommandHandler<DeleteColor> deleteColor,
        int id)
    {
        if (getColorById.Handle(new GetColorById(id)) is null)
        {
            return RestResults.NotFound("Color", id);
        }

        deleteColor.Handle(new DeleteColor(id));
        return TypedResults.NoContent();
    }
}
