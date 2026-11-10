using Rentar.Shared.Kernel.Entities;
using System;

namespace Rentar.Modules.Rentals.Domain;

public class Rental : IEntity
{
    public int Id { get; set; }

    /// <summary>Id of the Fleet car; a reference across modules, so not a foreign key.</summary>
    public int CarId { get; set; }

    /// <summary>Id of the Customers customer; a reference across modules, so not a foreign key.</summary>
    public int CustomerId { get; set; }

    public DateTime RentDate { get; set; }
    public DateTime ReturnDate { get; set; }
}
