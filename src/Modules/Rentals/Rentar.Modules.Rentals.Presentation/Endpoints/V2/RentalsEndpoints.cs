#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Rentals.Application;
using Rentar.Modules.Rentals.Domain;
using Rentar.Shared.Presentation.Endpoints;
using System;
using System.Collections.Generic;

namespace Rentar.Modules.Rentals.Presentation.Endpoints.V2;

public class RentalsEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Rentals", "rentals", 2.0);

        group.MapGet("/", GetAll);
        group.MapGet("/{id:int}", GetById)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapGet("/details", GetDetails)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);
        group.MapGet("/availability", GetAvailability);
        group.MapPost("/", Create)
            .ProducesValidationProblem();
    }

    /// <summary>
    /// Lists rentals, optionally only those of a car.
    /// </summary>
    private static Ok<List<Rental>> GetAll(IRentalService rentalService, int? carId)
    {
        var rentals = carId is { } car
            ? rentalService.GetRentalsByCarId(car)
            : rentalService.GetAll();

        return TypedResults.Ok(rentals);
    }

    private static Results<Ok<Rental>, ProblemHttpResult> GetById(IRentalService rentalService, int id)
    {
        var rental = rentalService.GetById(id);
        return rental is null ? RestResults.NotFound("Rental", id) : TypedResults.Ok(rental);
    }

    /// <summary>
    /// Rentals together with their car, customer and user; a rental whose car, customer or user is gone is left out.
    /// Closed during the 22:00 maintenance hour.
    /// </summary>
    private static Ok<List<RentalDetailDto>> GetDetails(IRentalService rentalService)
    {
        return TypedResults.Ok(rentalService.GetRentalDetails());
    }

    private static Ok<RentalAvailabilityResponse> GetAvailability(
        IRentalService rentalService,
        int carId,
        DateTime rentDate,
        DateTime returnDate)
    {
        var available = rentalService.IsRentable(new Rental { CarId = carId, RentDate = rentDate, ReturnDate = returnDate });
        return TypedResults.Ok(new RentalAvailabilityResponse(carId, rentDate, returnDate, available));
    }

    private static Created<Rental> Create(IRentalService rentalService, HttpRequest request, RentalRequest body)
    {
        var rental = new Rental
        {
            CarId = body.CarId,
            CustomerId = body.CustomerId,
            RentDate = body.RentDate,
            ReturnDate = body.ReturnDate
        };

        rentalService.Add(rental);
        return RestResults.Created(request, rental.Id, rental);
    }
}
