using Rentar.Modules.Rentals.Domain;
using Rentar.Shared.Application.Data;

namespace Rentar.Modules.Rentals.Application;

public interface IRentalDal : IEntityRepository<Rental>
{
}
