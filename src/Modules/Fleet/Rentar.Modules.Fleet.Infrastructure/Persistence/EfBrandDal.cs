using Microsoft.EntityFrameworkCore;
using Rentar.Modules.Fleet.Application.Brands;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Infrastructure.Persistence;

namespace Rentar.Modules.Fleet.Infrastructure.Persistence;

public class EfBrandDal(IDbContextFactory<FleetDbContext> contextFactory) : EfEntityRepositoryBase<Brand, FleetDbContext>(contextFactory), IBrandDal
{
}
