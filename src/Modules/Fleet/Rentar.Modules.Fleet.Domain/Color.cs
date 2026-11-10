using Rentar.Shared.Kernel.Entities;

namespace Rentar.Modules.Fleet.Domain;

public class Color : IEntity
{
    public int Id { get; set; }
    public string ColorName { get; set; }
}
