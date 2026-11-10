using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Rentar.Shared.Application.Caching;
using Rentar.Shared.Application.Interceptors;
using Rentar.Shared.Application.IoC;
using System.Linq;

namespace Rentar.Shared.Application.Aspects.Caching;

public class CacheAspect(int duration = 60) : MethodInterception
{
    private readonly int _duration = duration;
    private readonly ICacheManager _cacheManager = ServiceTool.ServiceProvider.GetRequiredService<ICacheManager>();

    public override void Intercept(IInvocation invocation)
    {
        // Keyed by the handler class (e.g. "...Cars.GetCarsByBrandHandler.Handle") rather than the shared handler
        // interface, so a CacheRemoveAspect pattern can name the use cases it invalidates.
        var methodName = $"{(invocation.TargetType ?? invocation.Method.ReflectedType).FullName}.{invocation.Method.Name}";
        var arguments = invocation.Arguments.ToList();
        var key = $"{methodName}({string.Join(",", arguments.Select(x => x?.ToString() ?? "<Null>"))})";
        if (_cacheManager.IsAdd(key))
        {
            invocation.ReturnValue = _cacheManager.Get(key);
            return;
        }
        invocation.Proceed();
        _cacheManager.Add(key, invocation.ReturnValue, _duration);
    }
}
