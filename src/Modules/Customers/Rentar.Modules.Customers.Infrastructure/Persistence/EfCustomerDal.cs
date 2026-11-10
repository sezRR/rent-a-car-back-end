using Microsoft.EntityFrameworkCore;
using Rentar.Modules.Customers.Application;
using Rentar.Modules.Customers.Domain;
using Rentar.Shared.Infrastructure.Persistence;

namespace Rentar.Modules.Customers.Infrastructure.Persistence;

public class EfCustomerDal(IDbContextFactory<CustomersDbContext> contextFactory) : EfEntityRepositoryBase<Customer, CustomersDbContext>(contextFactory), ICustomerDal
{
}
