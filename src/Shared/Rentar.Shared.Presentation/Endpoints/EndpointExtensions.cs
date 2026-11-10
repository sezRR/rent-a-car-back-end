using Asp.Versioning;
using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;
using System.Reflection;

namespace Rentar.Shared.Presentation.Endpoints;

public static class EndpointExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers every <see cref="IEndpoint"/> declared in a module's Presentation assembly.
        /// </summary>
        public IServiceCollection AddEndpoints(Assembly assembly)
        {
            var endpointTypes = assembly.GetTypes()
                .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

            foreach (var endpointType in endpointTypes)
            {
                services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IEndpoint), endpointType));
            }

            return services;
        }
    }

    extension(WebApplication app)
    {
        /// <summary>
        /// Maps the endpoints every module registered.
        /// </summary>
        public void MapEndpoints()
        {
            foreach (var endpoint in app.Services.GetServices<IEndpoint>())
            {
                endpoint.MapEndpoints(app);
            }
        }
    }

    extension(IEndpointRouteBuilder builder)
    {
        /// <summary>
        /// Opens a versioned route group at <c>/api/v{version}/{prefix}</c>, tagged with
        /// <paramref name="name"/> so the OpenAPI document groups its endpoints together.
        /// </summary>
        public RouteGroupBuilder MapVersionedGroup(string name, string prefix, double version)
        {
            return builder.NewVersionedApi(name)
                .MapGroup($"/api/v{{version:apiVersion}}/{prefix}")
                .HasApiVersion(new ApiVersion(version))
                .WithTags(name);
        }
    }
}
