#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Customers.Application;
using Rentar.Modules.Customers.Domain;
using Rentar.Shared.Presentation.Endpoints;
using System.Collections.Generic;
using KernelResult = Rentar.Shared.Kernel.Results.IResult;

namespace Rentar.Modules.Customers.Presentation.Endpoints.V2;

public class CustomersEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Customers", "customers", 2.0);

        group.MapGet("/", GetAll)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);
        group.MapGet("/{id:int}", GetById)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPost("/", Create)
            .ProducesValidationProblem();
    }

    /// <summary>
    /// Lists customers, optionally only those of a user.
    /// </summary>
    private static Results<Ok<List<Customer>>, ProblemHttpResult> GetAll(ICustomerService customerService, int? userId)
    {
        var result = userId is { } user
            ? customerService.GetCustomersByUserId(user)
            : customerService.GetAll();

        return result.Success ? TypedResults.Ok(result.Data) : Problem(result);
    }

    private static Results<Ok<Customer>, ProblemHttpResult> GetById(ICustomerService customerService, int id)
    {
        var customer = customerService.GetById(id).Data;
        return customer is null ? RestResults.NotFound("Customer", id) : TypedResults.Ok(customer);
    }

    private static Results<Created<Customer>, ProblemHttpResult> Create(
        ICustomerService customerService,
        HttpRequest request,
        CustomerRequest body)
    {
        var customer = new Customer { UserId = body.UserId, CompanyName = body.CompanyName };

        var result = customerService.Add(customer);
        return result.Success ? RestResults.Created(request, customer.Id, customer) : Problem(result);
    }

    /// <summary>
    /// The customer list is closed during the 22:00 maintenance hour, which is a temporary outage rather than a bad request.
    /// </summary>
    private static ProblemHttpResult Problem(KernelResult result)
    {
        return RestResults.Problem(result, result.Message == Messages.MaintenanceTime
            ? StatusCodes.Status503ServiceUnavailable
            : StatusCodes.Status400BadRequest);
    }
}
