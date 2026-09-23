using System;
using System.Linq;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebAPI.Common;

public static class EndpointExtensions
{
    extension(WebApplication app)
    {
        public void MapAllEndpoints()
        {
            var modules = typeof(Program).Assembly.GetTypes()
                .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var module in modules)
            {
                var instance = (IEndpoint)Activator.CreateInstance(module)!;
                instance.MapEndpoints(app);
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
