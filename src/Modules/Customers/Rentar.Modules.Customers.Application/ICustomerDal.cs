using Rentar.Modules.Customers.Domain;
using Rentar.Shared.Application.Data;

namespace Rentar.Modules.Customers.Application;

public interface ICustomerDal : IEntityRepository<Customer>
{
}
