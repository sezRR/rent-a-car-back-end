using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WebAPI.Common;

namespace WebAPI.Endpoints.v1;

public class CustomersEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Customers", "customers", 1.0);

        group.MapGet("/getall", GetAll);
        group.MapGet("/getcustomersbyuserid", GetCustomersByUserId);
        group.MapPost("/add", Add);
    }

    private IResult GetAll(ICustomerService customerService)
    {
        var result = customerService.GetAll();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult GetCustomersByUserId(ICustomerService customerService, int userId)
    {
        var result = customerService.GetCustomersByUserId(userId);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Add(ICustomerService customerService, Customer customer)
    {
        var result = customerService.Add(customer);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
