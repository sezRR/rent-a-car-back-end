using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rentar.Shared.Infrastructure.Modules;

/// <summary>
/// The single entry point a module offers the host. Everything else in a module stays behind it.
/// </summary>
public interface IModule
{
    string Name { get; }

    /// <summary>Framework registrations: the module's context, options, endpoints.</summary>
    void AddServices(IServiceCollection services, IConfiguration configuration);

    /// <summary>Autofac registrations: the module's services, wrapped by the aspect interceptors where needed.</summary>
    void ConfigureContainer(ContainerBuilder builder);
}
