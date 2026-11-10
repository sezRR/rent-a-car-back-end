using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Rentar.Shared.Application.Caching;
using Rentar.Shared.Application.Interceptors;
using Rentar.Shared.Application.IoC;

namespace Rentar.Shared.Application.Aspects.Caching;

public class CacheRemoveAspect(string pattern) : MethodInterception
{
    private readonly string _pattern = pattern;
    private readonly ICacheManager _cacheManager = ServiceTool.ServiceProvider.GetRequiredService<ICacheManager>();

    protected override void OnSuccess(IInvocation invocation)
    {
        _cacheManager.RemoveByPattern(_pattern);
    }
}
