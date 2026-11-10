using System;

namespace Rentar.Modules.Rentals.Presentation.Endpoints.V2;

/// <summary>
/// Whether a car is free for the whole period, i.e. no rental of it overlaps the period.
/// </summary>
public sealed record RentalAvailabilityResponse(int CarId, DateTime RentDate, DateTime ReturnDate, bool Available);
