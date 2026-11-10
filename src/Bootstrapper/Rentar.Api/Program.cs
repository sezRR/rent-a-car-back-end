using Asp.Versioning;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rentar.Api.OpenApi;
using Rentar.Modules.Customers.Infrastructure;
using Rentar.Modules.Findeks.Infrastructure;
using Rentar.Modules.Fleet.Infrastructure;
using Rentar.Modules.Identity.Infrastructure;
using Rentar.Modules.Payments.Infrastructure;
using Rentar.Modules.Rentals.Infrastructure;
using Rentar.Shared.Application.IoC;
using Rentar.Shared.Infrastructure;
using Rentar.Shared.Infrastructure.Modules;
using Rentar.Shared.Infrastructure.Persistence;
using Rentar.Shared.Presentation.Endpoints;
using Rentar.Shared.Presentation.Errors;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Each module registers its own context, services and endpoints; the host only composes them.
IModule[] modules =
[
    new IdentityModule(),
    new FleetModule(),
    new CustomersModule(),
    new RentalsModule(),
    new PaymentsModule(),
    new FindeksModule()
];

builder.Services.AddSharedInfrastructure();

foreach (var module in modules)
{
    module.AddServices(builder.Services, builder.Configuration);
}

builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(container =>
    {
        foreach (var module in modules)
        {
            module.ConfigureContainer(container);
        }
    });

builder.Services.AddAuthorization();

builder.Services.AddCors();
builder.Services.AddVersionedOpenApi(1, 2);
builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
    options.ReportApiVersions = true;
});

var app = builder.Build();

ServiceTool.Initialize(app.Services);

if (app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    await app.Services.MigrateModuleDatabasesAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi().AllowAnonymous();
    app.MapScalarApiReference("/docs", options => options
            .AddDocument("v2", "Rentar API v2", isDefault: true)
            .AddDocument("v1", "Rentar API v1"))
        .AllowAnonymous();
}

app.UseCustomExceptionMiddleware();

app.UseCors(cors => cors.WithOrigins("http://localhost:4200").AllowAnyHeader());

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapEndpoints();

await app.RunAsync();

/// <summary>
/// Exposed so <c>WebApplicationFactory&lt;Program&gt;</c> in the integration tests can boot this host.
/// </summary>
public partial class Program;
