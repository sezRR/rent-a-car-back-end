using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Rentals.Application;
using Rentar.Modules.Rentals.Domain;
using Rentar.Shared.Presentation.Endpoints;
using System.Collections.Generic;

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

    private static Ok<V1Response<List<Rental>>> GetAll(IRentalService rentalService)
    {
        return V1Results.Ok(rentalService.GetAll(), Messages.RentalsListed);
    }

    private static Ok<V1Response<Rental>> GetRentalByCarId(IRentalService rentalService, int carId)
    {
        return V1Results.Ok(rentalService.GetRentalByCarId(carId), null);
    }

    private static Ok<V1Response<List<RentalDetailDto>>> GetRentalDetails(IRentalService rentalService)
    {
        return V1Results.Ok(rentalService.GetRentalDetails(), null);
    }

    private static Results<Ok<V1Response>, BadRequest<V1Response>> IsRentable(IRentalService rentalService, Rental rental)
    {
        return rentalService.IsRentable(rental)
            ? V1Results.Ok(null)
            : V1Results.Fail(Messages.CarNotAvailable);
    }

    private static Ok<V1Response> Add(IRentalService rentalService, Rental rental)
    {
        rentalService.Add(rental);
        return V1Results.Ok(Messages.RentalAdded);
    }
}
