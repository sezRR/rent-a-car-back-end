using Rentar.Shared.Kernel.Entities;

namespace Rentar.Modules.Identity.Domain;

public class OperationClaim : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}
