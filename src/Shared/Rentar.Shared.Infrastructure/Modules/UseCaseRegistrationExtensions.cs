using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Rentar.Shared.Application.Interceptors;
using Rentar.Shared.Application.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rentar.Shared.Infrastructure.Modules;

public static class UseCaseRegistrationExtensions
{
    private static readonly ProxyGenerationOptions ProxyOptions = new()
    {
        Selector = new AspectInterceptorSelector()
    };

    private static readonly Type[] HandlerInterfaces =
    [
        typeof(ICommandHandler<>),
        typeof(ICommandHandler<,>),
        typeof(IQueryHandler<,>)
    ];

    /// <summary>
    /// Registers every use case handler in a module's Application assembly behind an interface proxy, so the aspect
    /// attributes on its <c>Handle</c> method (validation, caching, transactions, ...) run around each call.
    /// </summary>
    public static void RegisterUseCases(this ContainerBuilder builder, Assembly applicationAssembly)
    {
        builder.RegisterAssemblyTypes(applicationAssembly)
            .Where(type => HandledInterfacesOf(type).Any())
            .As(HandledInterfacesOf)
            .EnableInterfaceInterceptors(ProxyOptions)
            .SingleInstance();
    }

    /// <summary>
    /// The closed handler interfaces a type implements, e.g. <c>IQueryHandler&lt;GetBrands, List&lt;Brand&gt;&gt;</c>.
    /// </summary>
    public static IEnumerable<Type> HandledInterfacesOf(Type type)
    {
        return type.GetInterfaces()
            .Where(i => i.IsGenericType && HandlerInterfaces.Contains(i.GetGenericTypeDefinition()));
    }
}
