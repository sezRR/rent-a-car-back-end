using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Customers.Application;
using Rentar.Modules.Customers.Domain;
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

    private static Ok<V1Response<List<Customer>>> GetAll(ICustomerService customerService)
    {
        return V1Results.Ok(customerService.GetAll(), Messages.CustomersListed);
    }

    private static Ok<V1Response<List<Customer>>> GetCustomersByUserId(ICustomerService customerService, int userId)
    {
        return V1Results.Ok(customerService.GetCustomersByUserId(userId), null);
    }

    private static Ok<V1Response> Add(ICustomerService customerService, Customer customer)
    {
        customerService.Add(customer);
        return V1Results.Ok(Messages.CustomerAdded);
    }
}
