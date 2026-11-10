#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Payments.Application;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Presentation.Endpoints;

namespace Rentar.Modules.Payments.Presentation.Endpoints.V2;

public class PaymentsEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Payments", "payments", 2.0);

        group.MapPost("/", Create)
            .ProducesProblem(StatusCodes.Status402PaymentRequired);
    }

    /// <summary>
    /// Takes a payment. A declined payment answers 402 Payment Required.
    /// </summary>
    private static Results<NoContent, ProblemHttpResult> Create(ICommandHandler<TakePayment, bool> takePayment)
    {
        return takePayment.Handle(new TakePayment())
            ? TypedResults.NoContent()
            : TypedResults.Problem(detail: Messages.PaymentFailed, statusCode: StatusCodes.Status402PaymentRequired);
    }
}
