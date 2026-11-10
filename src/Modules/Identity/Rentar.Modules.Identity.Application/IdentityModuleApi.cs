using Rentar.Modules.Identity.Application.Users;
using Rentar.Modules.Identity.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Rentar.Modules.Identity.Application;

public class IdentityModuleApi(IUserDal userDal) : IIdentityModuleApi
{
    private readonly IUserDal _userDal = userDal;

    public IReadOnlyList<UserSummary> GetUsers(IReadOnlyCollection<int> userIds)
    {
        if (userIds.Count == 0)
        {
            return [];
        }

        var ids = userIds.ToArray();
        return _userDal.GetAll(u => ids.Contains(u.Id))
            .Select(u => new UserSummary(u.Id, u.FirstName, u.LastName))
            .ToList();
    }
}
