using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application;
using Rentar.Modules.Fleet.Application.Colors;
using Rentar.Modules.Fleet.Domain;
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

    private static Ok<V1Response<List<Color>>> GetAll(IColorService colorService)
    {
        return V1Results.Ok(colorService.GetAll(), Messages.ColorsListed);
    }

    private static Ok<V1Response<Color>> GetById(IColorService colorService, int id)
    {
        return V1Results.Ok(colorService.GetById(id), null);
    }

    private static Ok<V1Response> Add(IColorService colorService, Color color)
    {
        colorService.Add(color);
        return V1Results.Ok(Messages.ColorAdded);
    }

    private static Ok<V1Response> Update(IColorService colorService, Color color)
    {
        colorService.Update(color);
        return V1Results.Ok(Messages.ColorUpdated);
    }

    private static Ok<V1Response> Delete(IColorService colorService, Color color)
    {
        colorService.Delete(color);
        return V1Results.Ok(Messages.ColorDeleted);
    }
}
