using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rentar.Modules.Rentals.Application;
using Rentar.Modules.Rentals.Contracts;
using Rentar.Modules.Rentals.Infrastructure.Persistence;
using Rentar.Shared.Infrastructure.Modules;
using Rentar.Shared.Infrastructure.Persistence;
using Rentar.Shared.Presentation.Endpoints;

namespace Rentar.Modules.Rentals.Infrastructure;

/// <summary>
/// Rentals of Fleet cars by customers. Reads the other modules only through their contracts.
/// </summary>
public sealed class RentalsModule : IModule
{
    public string Name => "Rentals";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<RentalsDbContext>(RentalsDbContext.Schema);
        services.AddEndpoints(Presentation.AssemblyReference.Assembly);
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<EfRentalDal>().As<IRentalDal>().SingleInstance();
        builder.RegisterType<RentalsModuleApi>().As<IRentalsModuleApi>().SingleInstance();

        builder.RegisterAspectService<RentalManager, IRentalService>();
    }
}
