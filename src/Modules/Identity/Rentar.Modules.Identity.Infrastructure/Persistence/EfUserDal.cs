using Microsoft.EntityFrameworkCore;
using Rentar.Modules.Identity.Application.Users;
using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;

namespace Rentar.Modules.Identity.Infrastructure.Persistence;

public class EfUserDal(IDbContextFactory<IdentityDbContext> contextFactory) : EfEntityRepositoryBase<User, IdentityDbContext>(contextFactory), IUserDal
{
    public List<OperationClaim> GetClaims(User user)
    {
        using var context = ContextFactory.CreateDbContext();
        var result = from operationClaim in context.OperationClaims
                     join userOperationClaim in context.UserOperationClaims
                         on operationClaim.Id equals userOperationClaim.OperationClaimId
                     where userOperationClaim.UserId == user.Id
                     select new OperationClaim { Id = operationClaim.Id, Name = operationClaim.Name };
        return result.ToList();
    }
}
