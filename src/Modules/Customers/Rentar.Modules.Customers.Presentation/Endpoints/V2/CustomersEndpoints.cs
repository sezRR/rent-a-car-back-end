#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Customers.Application;
using Rentar.Modules.Customers.Domain;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Presentation.Endpoints;
using System.Collections.Generic;

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
    /// Lists customers, optionally only those of a user. The full list is closed during the 22:00 maintenance hour.
    /// </summary>
    private static Ok<List<Customer>> GetAll(
        IQueryHandler<GetCustomers, List<Customer>> getCustomers,
        IQueryHandler<GetCustomersByUser, List<Customer>> getCustomersByUser,
        int? userId)
    {
        var customers = userId is { } user
            ? getCustomersByUser.Handle(new GetCustomersByUser(user))
            : getCustomers.Handle(new GetCustomers());

        return TypedResults.Ok(customers);
    }

    private static Results<Ok<Customer>, ProblemHttpResult> GetById(IQueryHandler<GetCustomerById, Customer> getCustomerById, int id)
    {
        var customer = getCustomerById.Handle(new GetCustomerById(id));
        return customer is null ? RestResults.NotFound("Customer", id) : TypedResults.Ok(customer);
    }

    private static Created<Customer> Create(
        ICommandHandler<CreateCustomer, Customer> createCustomer,
        HttpRequest request,
        CustomerRequest body)
    {
        var customer = createCustomer.Handle(new CreateCustomer(body.UserId, body.CompanyName));
        return RestResults.Created(request, customer.Id, customer);
    }
}
