using Microsoft.Extensions.DependencyInjection;
using Rentar.Shared.Application.Caching;
using Rentar.Shared.Application.Security;
using Rentar.Shared.Infrastructure.Caching;
using Rentar.Shared.Infrastructure.Security;
using System;
using System.Diagnostics;

namespace Rentar.Shared.Infrastructure;

public static class SharedInfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// Services the shared aspects resolve, plus the clock modules read the time from.
    /// </summary>
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheManager, MemoryCacheManager>();
        services.AddHttpContextAccessor();
        services.AddSingleton<ICurrentUser, HttpCurrentUser>();
        services.AddSingleton<Stopwatch>();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
