using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rentar.Modules.Payments.Application;
using Rentar.Modules.Payments.Contracts;
using Rentar.Shared.Infrastructure.Modules;
using Rentar.Shared.Presentation.Endpoints;

namespace Rentar.Modules.Payments.Infrastructure;

/// <summary>
/// Taking payments. Stateless, so it owns no schema.
/// </summary>
public sealed class PaymentsModule : IModule
{
    public string Name => "Payments";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpoints(Presentation.AssemblyReference.Assembly);
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<PaymentsModuleApi>().As<IPaymentsModuleApi>().SingleInstance();

        builder.RegisterAspectService<PaymentManager, IPaymentService>();
    }
}
