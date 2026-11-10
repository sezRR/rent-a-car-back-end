using Rentar.Modules.Customers.Domain;
using System.Collections.Generic;

namespace Rentar.Modules.Customers.Application;

public interface ICustomerService
{
    /// <exception cref="Rentar.Shared.Kernel.Exceptions.UnavailableException">It is the 22:00 maintenance hour.</exception>
    List<Customer> GetAll();

    /// <returns>The customer, or <c>null</c> when there is none with that id.</returns>
    Customer GetById(int id);

    List<Customer> GetCustomersByUserId(int userId);

    void Add(Customer customer);

    /// <exception cref="Rentar.Shared.Kernel.Exceptions.BusinessRuleException">The company name is shorter than two characters.</exception>
    void Update(Customer customer);

    /// <exception cref="Rentar.Shared.Kernel.Exceptions.BusinessRuleException">The company name is shorter than two characters.</exception>
    void Delete(Customer customer);
}
