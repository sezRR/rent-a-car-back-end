using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application;
using Rentar.Modules.Fleet.Application.Colors;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Presentation.Endpoints;
using System.Collections.Generic;

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

    private static Ok<V1Response<List<Color>>> GetAll(IQueryHandler<GetColors, List<Color>> getColors)
    {
        return V1Results.Ok(getColors.Handle(new GetColors()), Messages.ColorsListed);
    }

    private static Ok<V1Response<Color>> GetById(IQueryHandler<GetColorById, Color> getColorById, int id)
    {
        return V1Results.Ok(getColorById.Handle(new GetColorById(id)), null);
    }

    private static Ok<V1Response> Add(ICommandHandler<CreateColor, Color> createColor, CreateColor command)
    {
        createColor.Handle(command);
        return V1Results.Ok(Messages.ColorAdded);
    }

    private static Ok<V1Response> Update(ICommandHandler<UpdateColor> updateColor, UpdateColor command)
    {
        updateColor.Handle(command);
        return V1Results.Ok(Messages.ColorUpdated);
    }

    private static Ok<V1Response> Delete(ICommandHandler<DeleteColor> deleteColor, DeleteColor command)
    {
        deleteColor.Handle(command);
        return V1Results.Ok(Messages.ColorDeleted);
    }
}
