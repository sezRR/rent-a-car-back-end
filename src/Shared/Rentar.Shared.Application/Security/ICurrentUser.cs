using System.Collections.Generic;

namespace Rentar.Shared.Application.Security;

/// <summary>
/// The caller of the current operation, so application code can authorize without knowing about HTTP.
/// </summary>
public interface ICurrentUser
{
    IReadOnlyCollection<string> Roles { get; }
}
