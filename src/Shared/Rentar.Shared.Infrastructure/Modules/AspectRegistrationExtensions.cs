using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Rentar.Shared.Application.Interceptors;

namespace Rentar.Shared.Infrastructure.Modules;

public static class AspectRegistrationExtensions
{
    private static readonly ProxyGenerationOptions ProxyOptions = new()
    {
        Selector = new AspectInterceptorSelector()
    };

    /// <summary>
    /// Registers an application service behind an interface proxy, so the aspect attributes on it
    /// (validation, caching, transactions, ...) run around each call.
    /// </summary>
    public static void RegisterAspectService<TImplementation, TService>(this ContainerBuilder builder)
        where TImplementation : notnull, TService
        where TService : notnull
    {
        builder.RegisterType<TImplementation>()
            .As<TService>()
            .EnableInterfaceInterceptors(ProxyOptions)
            .SingleInstance();
    }
}
