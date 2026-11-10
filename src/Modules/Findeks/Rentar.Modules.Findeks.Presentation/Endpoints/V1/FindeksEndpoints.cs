using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
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

    private static Ok<V1Response<int>> CalculateFindeksRating(IFindeksService findeksService)
    {
        return V1Results.Ok(findeksService.CalculateFindeksRating(), Messages.FindeksRatingCalculated);
    }
}
