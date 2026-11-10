using Microsoft.EntityFrameworkCore.Design;
using Rentar.Shared.Infrastructure.Persistence;

namespace Rentar.Modules.Customers.Infrastructure.Persistence;

internal sealed class CustomersDbContextDesignTimeFactory : IDesignTimeDbContextFactory<CustomersDbContext>
{
    public CustomersDbContext CreateDbContext(string[] args)
    {
        return new CustomersDbContext(ModuleDbContextOptions.ForDesignTime<CustomersDbContext>(CustomersDbContext.Schema));
    }
}
