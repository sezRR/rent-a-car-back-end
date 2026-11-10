using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Customers.Application;
using Rentar.Modules.Customers.Domain;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Presentation.Endpoints;
using System.Collections.Generic;

namespace Rentar.Modules.Customers.Presentation.Endpoints.V1;

public class CustomersEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Customers", "customers", 1.0);

        group.MapGet("/getall", GetAll);
        group.MapGet("/getcustomersbyuserid", GetCustomersByUserId);
        group.MapPost("/add", Add);
    }

    private static Ok<V1Response<List<Customer>>> GetAll(IQueryHandler<GetCustomers, List<Customer>> getCustomers)
    {
        return V1Results.Ok(getCustomers.Handle(new GetCustomers()), Messages.CustomersListed);
    }

    private static Ok<V1Response<List<Customer>>> GetCustomersByUserId(
        IQueryHandler<GetCustomersByUser, List<Customer>> getCustomersByUser,
        int userId)
    {
        return V1Results.Ok(getCustomersByUser.Handle(new GetCustomersByUser(userId)), null);
    }

    private static Ok<V1Response> Add(ICommandHandler<CreateCustomer, Customer> createCustomer, CreateCustomer command)
    {
        createCustomer.Handle(command);
        return V1Results.Ok(Messages.CustomerAdded);
    }
}
