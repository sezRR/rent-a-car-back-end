using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Rentar.Shared.Application.Interceptors;
using Rentar.Shared.Application.IoC;
using Rentar.Shared.Application.Security;
using System;
using System.Linq;

namespace Rentar.Shared.Application.Aspects.Security;

/// <summary>
/// Lets the call through only when the current user holds one of the comma separated roles.
/// </summary>
public class SecuredOperation(string roles) : MethodInterception
{
    public const string AuthorizationDenied = "You do not have permission";

    private readonly string[] _roles = roles.Split(',');

    protected override void OnBefore(IInvocation invocation)
    {
        var currentUser = ServiceTool.ServiceProvider.GetRequiredService<ICurrentUser>();
        if (_roles.Any(currentUser.Roles.Contains))
        {
            return;
        }

        throw new UnauthorizedAccessException(AuthorizationDenied);
    }
}
