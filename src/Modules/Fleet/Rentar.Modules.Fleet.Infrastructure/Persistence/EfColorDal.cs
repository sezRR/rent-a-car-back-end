using Microsoft.EntityFrameworkCore;
using Rentar.Modules.Fleet.Application.Colors;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Infrastructure.Persistence;

namespace Rentar.Modules.Fleet.Infrastructure.Persistence;

public class EfColorDal(IDbContextFactory<FleetDbContext> contextFactory) : EfEntityRepositoryBase<Color, FleetDbContext>(contextFactory), IColorDal
{
}
