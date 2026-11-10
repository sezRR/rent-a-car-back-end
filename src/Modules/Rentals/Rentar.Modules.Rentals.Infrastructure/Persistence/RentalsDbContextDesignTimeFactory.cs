using Microsoft.EntityFrameworkCore.Design;
using Rentar.Shared.Infrastructure.Persistence;

namespace Rentar.Modules.Rentals.Infrastructure.Persistence;

internal sealed class RentalsDbContextDesignTimeFactory : IDesignTimeDbContextFactory<RentalsDbContext>
{
    public RentalsDbContext CreateDbContext(string[] args)
    {
        return new RentalsDbContext(ModuleDbContextOptions.ForDesignTime<RentalsDbContext>(RentalsDbContext.Schema));
    }
}
