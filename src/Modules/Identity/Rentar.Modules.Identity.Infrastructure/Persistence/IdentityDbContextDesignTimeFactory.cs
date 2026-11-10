using Microsoft.EntityFrameworkCore.Design;
using Rentar.Shared.Infrastructure.Persistence;

namespace Rentar.Modules.Identity.Infrastructure.Persistence;

internal sealed class IdentityDbContextDesignTimeFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        return new IdentityDbContext(ModuleDbContextOptions.ForDesignTime<IdentityDbContext>(IdentityDbContext.Schema));
    }
}
