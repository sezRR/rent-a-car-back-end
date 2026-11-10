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
using KernelResult = Rentar.Shared.Kernel.Results.IResult;

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
    private static Results<Ok<List<Rental>>, ProblemHttpResult> GetAll(IRentalService rentalService, int? carId)
    {
        var result = carId is { } car
            ? rentalService.GetRentalsByCarId(car)
            : rentalService.GetAll();

        return result.Success ? TypedResults.Ok(result.Data) : Problem(result);
    }

    private static Results<Ok<Rental>, ProblemHttpResult> GetById(IRentalService rentalService, int id)
    {
        var rental = rentalService.GetById(id).Data;
        return rental is null ? RestResults.NotFound("Rental", id) : TypedResults.Ok(rental);
    }

    /// <summary>
    /// Rentals together with their car, customer and user; a rental whose car, customer or user is gone is left out.
    /// </summary>
    private static Results<Ok<List<RentalDetailDto>>, ProblemHttpResult> GetDetails(IRentalService rentalService)
    {
        var result = rentalService.GetRentalDetails();
        return result.Success ? TypedResults.Ok(result.Data) : Problem(result);
    }

    private static Ok<RentalAvailabilityResponse> GetAvailability(
        IRentalService rentalService,
        int carId,
        DateTime rentDate,
        DateTime returnDate)
    {
        var result = rentalService.IsRentable(new Rental { CarId = carId, RentDate = rentDate, ReturnDate = returnDate });
        return TypedResults.Ok(new RentalAvailabilityResponse(carId, rentDate, returnDate, result.Success));
    }

    private static Results<Created<Rental>, ProblemHttpResult> Create(
        IRentalService rentalService,
        HttpRequest request,
        RentalRequest body)
    {
        var rental = new Rental
        {
            CarId = body.CarId,
            CustomerId = body.CustomerId,
            RentDate = body.RentDate,
            ReturnDate = body.ReturnDate
        };

        var result = rentalService.Add(rental);
        return result.Success ? RestResults.Created(request, rental.Id, rental) : Problem(result);
    }

    /// <summary>
    /// Rental details are closed during the 22:00 maintenance hour, which is a temporary outage rather than a bad request.
    /// </summary>
    private static ProblemHttpResult Problem(KernelResult result)
    {
        return RestResults.Problem(result, result.Message == Messages.MaintenanceTime
            ? StatusCodes.Status503ServiceUnavailable
            : StatusCodes.Status400BadRequest);
    }
}
