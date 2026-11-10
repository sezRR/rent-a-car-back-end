using Rentar.Shared.Kernel.Entities;

namespace Rentar.Modules.Fleet.Domain;

public class Brand : IEntity
{
    public int Id { get; set; }
    public string BrandName { get; set; }
}
