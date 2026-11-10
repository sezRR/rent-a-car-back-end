using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Rentar.Shared.Presentation.Errors;

public static class ErrorHandlingExtensions
{
    /// <summary>
    /// Registers ASP.NET Core's problem details and <see cref="ApiExceptionHandler"/>.
    /// </summary>
    public static IServiceCollection AddApiErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<ApiExceptionHandler>();
        return services;
    }

    /// <summary>
    /// Adds the exception handler middleware, which hands every exception to <see cref="ApiExceptionHandler"/>.
    /// </summary>
    public static IApplicationBuilder UseApiErrorHandling(this IApplicationBuilder app)
    {
        return app.UseExceptionHandler();
    }
}
