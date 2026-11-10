using Microsoft.EntityFrameworkCore;
using Rentar.Modules.Fleet.Application.CarImages;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Infrastructure.Persistence;

namespace Rentar.Modules.Fleet.Infrastructure.Persistence;

public class EfCarImageDal(IDbContextFactory<FleetDbContext> contextFactory) : EfEntityRepositoryBase<CarImage, FleetDbContext>(contextFactory), ICarImageDal
{
}
