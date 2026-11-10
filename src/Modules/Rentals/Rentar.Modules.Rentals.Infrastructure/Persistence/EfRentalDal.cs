using Microsoft.EntityFrameworkCore;
using Rentar.Modules.Rentals.Application;
using Rentar.Modules.Rentals.Domain;
using Rentar.Shared.Infrastructure.Persistence;

namespace Rentar.Modules.Rentals.Infrastructure.Persistence;

public class EfRentalDal(IDbContextFactory<RentalsDbContext> contextFactory) : EfEntityRepositoryBase<Rental, RentalsDbContext>(contextFactory), IRentalDal
{
}
