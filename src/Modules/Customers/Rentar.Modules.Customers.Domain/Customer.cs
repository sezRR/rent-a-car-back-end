using Rentar.Shared.Kernel.Entities;

namespace Rentar.Modules.Customers.Domain;

public class Customer : IEntity
{
    public int Id { get; set; }

    /// <summary>Id of the Identity user; a reference across modules, so not a foreign key.</summary>
    public int UserId { get; set; }

    public string CompanyName { get; set; }
}
