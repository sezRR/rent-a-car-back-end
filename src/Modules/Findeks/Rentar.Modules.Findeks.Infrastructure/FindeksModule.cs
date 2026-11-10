using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rentar.Modules.Findeks.Application;
using Rentar.Modules.Findeks.Contracts;
using Rentar.Shared.Infrastructure.Modules;
using Rentar.Shared.Presentation.Endpoints;

namespace Rentar.Modules.Findeks.Infrastructure;

/// <summary>
/// Findeks credit ratings. Stateless, so it owns no schema.
/// </summary>
public sealed class FindeksModule : IModule
{
    public string Name => "Findeks";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpoints(Presentation.AssemblyReference.Assembly);
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<FindeksModuleApi>().As<IFindeksModuleApi>().SingleInstance();

        builder.RegisterUseCases(typeof(CalculateFindeksRating).Assembly);
    }
}
