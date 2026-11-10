using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Rentals.Application;
using Rentar.Modules.Rentals.Domain;
using Rentar.Shared.Presentation.Endpoints;

namespace Rentar.Modules.Rentals.Presentation.Endpoints.V1;

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

    private static IResult GetAll(IRentalService rentalService)
    {
        var result = rentalService.GetAll();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult GetRentalByCarId(IRentalService rentalService, int carId)
    {
        var result = rentalService.GetRentalByCarId(carId);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult GetRentalDetails(IRentalService rentalService)
    {
        var result = rentalService.GetRentalDetails();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult IsRentable(IRentalService rentalService, Rental rental)
    {
        var result = rentalService.IsRentable(rental);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult Add(IRentalService rentalService, Rental rental)
    {
        var result = rentalService.Add(rental);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
