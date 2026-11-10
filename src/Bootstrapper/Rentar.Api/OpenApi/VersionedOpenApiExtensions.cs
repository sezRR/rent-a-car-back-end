using System.Linq;
using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Rentar.Api.OpenApi;

public static class VersionedOpenApiExtensions
{
    /// <summary>
    /// Adds one OpenAPI document per major API version, named <c>v{major}</c>, holding only that version's endpoints.
    /// </summary>
    public static IServiceCollection AddVersionedOpenApi(this IServiceCollection services, params int[] majorVersions)
    {
        foreach (var majorVersion in majorVersions)
        {
            var version = new ApiVersion(majorVersion, 0);

            services.AddOpenApi($"v{majorVersion}", options =>
            {
                options.ShouldInclude = description => description.ActionDescriptor.EndpointMetadata
                    .OfType<ApiVersionMetadata>()
                    .Any(metadata => metadata.IsMappedTo(version));

                options.AddDocumentTransformer<ApiVersionPathTransformer>();
            });
        }

        return services;
    }
}
