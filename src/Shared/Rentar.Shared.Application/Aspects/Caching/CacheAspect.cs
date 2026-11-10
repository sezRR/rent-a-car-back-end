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
        var methodName = $"{invocation.Method.ReflectedType.FullName}.{invocation.Method.Name}";
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
