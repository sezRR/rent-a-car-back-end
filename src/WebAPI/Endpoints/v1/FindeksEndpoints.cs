using Business.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WebAPI.Common;

namespace WebAPI.Endpoints.v1;

public class FindeksEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Findeks", "findeks", 1.0);

        group.MapGet("/calculate", CalculateFindeksRating);
    }

    private IResult CalculateFindeksRating(IFindeksService findeksService)
    {
        var result = findeksService.CalculateFindeksRating();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}