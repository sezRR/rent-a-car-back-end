using Rentar.Modules.Customers.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Rentar.Modules.Customers.Application;

public class CustomersModuleApi(ICustomerDal customerDal) : ICustomersModuleApi
{
    private readonly ICustomerDal _customerDal = customerDal;

    public IReadOnlyList<CustomerSummary> GetCustomers(IReadOnlyCollection<int> customerIds)
    {
        if (customerIds.Count == 0)
        {
            return [];
        }

        var ids = customerIds.ToArray();
        return _customerDal.GetAll(c => ids.Contains(c.Id))
            .Select(c => new CustomerSummary(c.Id, c.UserId, c.CompanyName))
            .ToList();
    }
}
