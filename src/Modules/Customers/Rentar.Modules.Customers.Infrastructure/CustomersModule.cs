using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rentar.Modules.Customers.Application;
using Rentar.Modules.Customers.Contracts;
using Rentar.Modules.Customers.Infrastructure.Persistence;
using Rentar.Shared.Infrastructure.Modules;
using Rentar.Shared.Infrastructure.Persistence;
using Rentar.Shared.Presentation.Endpoints;

namespace Rentar.Modules.Customers.Infrastructure;

/// <summary>
/// Customer accounts: the renting party, linked to an Identity user.
/// </summary>
public sealed class CustomersModule : IModule
{
    public string Name => "Customers";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<CustomersDbContext>(CustomersDbContext.Schema);
        services.AddEndpoints(Presentation.AssemblyReference.Assembly);
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<EfCustomerDal>().As<ICustomerDal>().SingleInstance();
        builder.RegisterType<CustomersModuleApi>().As<ICustomersModuleApi>().SingleInstance();

        builder.RegisterAspectService<CustomerManager, ICustomerService>();
    }
}
