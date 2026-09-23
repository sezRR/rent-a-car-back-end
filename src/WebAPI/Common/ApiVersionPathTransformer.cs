using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace WebAPI.Common;

/// <summary>
/// Endpoints are routed through the <c>v{version:apiVersion}</c> segment, which would otherwise
/// leave an unresolved <c>{version}</c> placeholder in the document. Each document describes a
/// single version, so the placeholder is baked out into the version that document is named after.
/// </summary>
public class ApiVersionPathTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var version = context.DocumentName.TrimStart('v', 'V');
        var paths = new OpenApiPaths();

        foreach (var (path, item) in document.Paths)
        {
            paths[path.Replace("{version}", version)] = item;
        }

        document.Paths = paths;
        return Task.CompletedTask;
    }
}
