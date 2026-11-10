using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Findeks.Application;
using Rentar.Shared.Presentation.Endpoints;

namespace Rentar.Modules.Findeks.Presentation.Endpoints.V1;

public class FindeksEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Findeks", "findeks", 1.0);

        group.MapGet("/calculate", CalculateFindeksRating);
    }

    private static IResult CalculateFindeksRating(IFindeksService findeksService)
    {
        var result = findeksService.CalculateFindeksRating();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
