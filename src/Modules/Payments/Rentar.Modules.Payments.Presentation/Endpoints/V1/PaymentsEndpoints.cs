using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Payments.Application;
using Rentar.Shared.Presentation.Endpoints;

namespace Rentar.Modules.Payments.Presentation.Endpoints.V1;

public class PaymentsEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Payments", "payments", 1.0);

        group.MapGet("/pay", Pay);
    }

    private static IResult Pay(IPaymentService paymentService)
    {
        var result = paymentService.Pay();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
