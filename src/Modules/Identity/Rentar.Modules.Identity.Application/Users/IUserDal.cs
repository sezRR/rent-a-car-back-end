using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Application.Data;
using System.Collections.Generic;

namespace Rentar.Modules.Identity.Application.Users;

public interface IUserDal : IEntityRepository<User>
{
    List<OperationClaim> GetClaims(User user);
}
