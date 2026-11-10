using Microsoft.EntityFrameworkCore.Design;
using Rentar.Shared.Infrastructure.Persistence;

namespace Rentar.Modules.Fleet.Infrastructure.Persistence;

internal sealed class FleetDbContextDesignTimeFactory : IDesignTimeDbContextFactory<FleetDbContext>
{
    public FleetDbContext CreateDbContext(string[] args)
    {
        return new FleetDbContext(ModuleDbContextOptions.ForDesignTime<FleetDbContext>(FleetDbContext.Schema));
    }
}
