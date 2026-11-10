using Rentar.Shared.Kernel.Entities;

namespace Rentar.Modules.Fleet.Domain;

public class Car : IEntity
{
    public int Id { get; set; }
    public int BrandId { get; set; }
    public int ColorId { get; set; }
    public int ModelYear { get; set; }
    public decimal DailyPrice { get; set; }
    public string Description { get; set; }
    public int MinimumFindeksRating { get; set; }
}
