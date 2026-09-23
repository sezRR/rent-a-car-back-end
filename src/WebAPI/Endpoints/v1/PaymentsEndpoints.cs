using Business.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WebAPI.Common;

namespace WebAPI.Endpoints.v1;

public class PaymentsEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Payments", "payments", 1.0);

        group.MapGet("/pay", Pay);
    }

    private IResult Pay(IPaymentService paymentService)
    {
        var result = paymentService.Pay();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
