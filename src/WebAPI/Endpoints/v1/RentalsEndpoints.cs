using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WebAPI.Common;

namespace WebAPI.Endpoints.v1;

public class RentalsEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Rentals", "rentals", 1.0);

        group.MapGet("/getall", GetAll);
        group.MapGet("/getrentalbycarid", GetRentalByCarId);
        group.MapGet("/getrentaldetails", GetRentalDetails);
        group.MapPost("/isrentable", IsRentable);
        group.MapPost("/add", Add);
    }

    private IResult GetAll(IRentalService rentalService)
    {
        var result = rentalService.GetAll();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult GetRentalByCarId(IRentalService rentalService, int carId)
    {
        var result = rentalService.GetRentalByCarId(carId);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult GetRentalDetails(IRentalService rentalService)
    {
        var result = rentalService.GetRentalDetails();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult IsRentable(IRentalService rentalService, Rental rental)
    {
        var result = rentalService.IsRentable(rental);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Add(IRentalService rentalService, Rental rental)
    {
        var result = rentalService.Add(rental);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
