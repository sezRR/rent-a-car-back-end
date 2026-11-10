using System;

namespace Rentar.Modules.Rentals.Presentation.Endpoints.V2;

/// <summary>
/// Body for creating a rental. The dates are local times, so send them without a UTC offset.
/// </summary>
public sealed record RentalRequest(int CarId, int CustomerId, DateTime RentDate, DateTime ReturnDate);
