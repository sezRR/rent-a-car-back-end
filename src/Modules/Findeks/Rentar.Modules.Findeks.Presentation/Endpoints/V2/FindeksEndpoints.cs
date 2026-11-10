#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Findeks.Application;
using Rentar.Shared.Presentation.Endpoints;

namespace Rentar.Modules.Findeks.Presentation.Endpoints.V2;

public class FindeksEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Findeks", "findeks", 2.0);

        group.MapGet("/rating", GetRating);
    }

    /// <summary>
    /// Calculates a Findeks rating. Calculating it changes nothing, so it is read with GET.
    /// </summary>
    private static Results<Ok<FindeksRatingResponse>, ProblemHttpResult> GetRating(IFindeksService findeksService)
    {
        var result = findeksService.CalculateFindeksRating();
        return result.Success
            ? TypedResults.Ok(new FindeksRatingResponse(result.Data))
            : RestResults.Problem(result);
    }
}
