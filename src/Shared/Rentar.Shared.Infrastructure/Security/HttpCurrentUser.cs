using Microsoft.AspNetCore.Http;
using Rentar.Shared.Application.Security;
using System;
using System.Collections.Generic;

namespace Rentar.Shared.Infrastructure.Security;

/// <summary>
/// Reads the caller from the JWT principal of the HTTP request being served.
/// </summary>
public class HttpCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public IReadOnlyCollection<string> Roles =>
        _httpContextAccessor.HttpContext?.User.ClaimRoles() ?? (IReadOnlyCollection<string>)Array.Empty<string>();
}
