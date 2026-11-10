using Rentar.Shared.Kernel.Entities;
using System;

namespace Rentar.Modules.Fleet.Domain;

public class CarImage : IEntity
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public string ImagePath { get; set; }
    public DateTime Date { get; set; }
}
