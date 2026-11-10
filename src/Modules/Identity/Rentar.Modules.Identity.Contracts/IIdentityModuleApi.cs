using System.Collections.Generic;

namespace Rentar.Modules.Identity.Contracts;

/// <summary>
/// What other modules may ask the Identity module.
/// </summary>
public interface IIdentityModuleApi
{
    /// <summary>Users with the given ids; unknown ids are left out.</summary>
    IReadOnlyList<UserSummary> GetUsers(IReadOnlyCollection<int> userIds);
}
