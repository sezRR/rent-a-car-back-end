using System;

namespace Rentar.Shared.Application.IoC;

/// <summary>
/// Gives aspect attributes, which the runtime instantiates without dependency injection, access to the
/// application's root service provider. The host sets it once the application has been built.
/// </summary>
public static class ServiceTool
{
    public static IServiceProvider ServiceProvider { get; private set; }

    public static void Initialize(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}
