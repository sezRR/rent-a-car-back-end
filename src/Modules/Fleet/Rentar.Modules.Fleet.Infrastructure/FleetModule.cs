using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rentar.Modules.Fleet.Application;
using Rentar.Modules.Fleet.Application.Brands;
using Rentar.Modules.Fleet.Application.CarImages;
using Rentar.Modules.Fleet.Application.Cars;
using Rentar.Modules.Fleet.Application.Colors;
using Rentar.Modules.Fleet.Contracts;
using Rentar.Modules.Fleet.Infrastructure.Persistence;
using Rentar.Modules.Fleet.Infrastructure.Storage;
using Rentar.Shared.Infrastructure.Modules;
using Rentar.Shared.Infrastructure.Persistence;
using Rentar.Shared.Presentation.Endpoints;

namespace Rentar.Modules.Fleet.Infrastructure;

/// <summary>
/// The cars on offer: brands, colors, cars and their images.
/// </summary>
public sealed class FleetModule : IModule
{
    public string Name => "Fleet";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<FleetDbContext>(FleetDbContext.Schema);
        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        services.AddOptions<ImageStorageOptions>().BindConfiguration(ImageStorageOptions.SectionName);
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<EfBrandDal>().As<IBrandDal>().SingleInstance();
        builder.RegisterType<EfColorDal>().As<IColorDal>().SingleInstance();
        builder.RegisterType<EfCarDal>().As<ICarDal>().SingleInstance();
        builder.RegisterType<EfCarImageDal>().As<ICarImageDal>().SingleInstance();
        builder.RegisterType<LocalImageStorage>().As<IImageStorage>().SingleInstance();
        builder.RegisterType<FleetModuleApi>().As<IFleetModuleApi>().SingleInstance();

        builder.RegisterUseCases(typeof(GetBrands).Assembly);
    }
}
