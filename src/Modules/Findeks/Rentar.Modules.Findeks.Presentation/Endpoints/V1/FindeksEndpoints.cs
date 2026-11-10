using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Findeks.Application;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Presentation.Endpoints;

namespace Rentar.Modules.Findeks.Presentation.Endpoints.V1;

public class FindeksEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Findeks", "findeks", 1.0);

        group.MapGet("/calculate", Calculate);
    }

    private static Ok<V1Response<int>> Calculate(IQueryHandler<CalculateFindeksRating, int> calculateFindeksRating)
    {
        return V1Results.Ok(calculateFindeksRating.Handle(new CalculateFindeksRating()), Messages.FindeksRatingCalculated);
    }
}
