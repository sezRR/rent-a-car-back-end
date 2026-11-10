using System.Collections.Generic;

namespace Rentar.Modules.Customers.Contracts;

/// <summary>
/// What other modules may ask the Customers module.
/// </summary>
public interface ICustomersModuleApi
{
    /// <summary>Customers with the given ids; unknown ids are left out.</summary>
    IReadOnlyList<CustomerSummary> GetCustomers(IReadOnlyCollection<int> customerIds);
}
