using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rentar.Shared.Presentation.Errors;

public class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(httpContext, e);
        }
    }

    private static Task HandleExceptionAsync(HttpContext httpContext, Exception e)
    {
        // v1 keeps its original error body; v2 onwards answers with RFC 9457 problem details.
        if (httpContext.ApiVersioningFeature.RequestedApiVersion?.MajorVersion >= 2)
        {
            return WriteProblemAsync(httpContext, e);
        }

        httpContext.Response.ContentType = "application/json";

        if (e is ValidationException validationException)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return httpContext.Response.WriteAsync(new ValidationErrorDetails
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = validationException.Message,
                Errors = validationException.Errors
            }.ToString());
        }

        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        return httpContext.Response.WriteAsync(new ErrorDetails
        {
            StatusCode = httpContext.Response.StatusCode,
            Message = "Internal Server Error"
        }.ToString());
    }

    private static Task WriteProblemAsync(HttpContext httpContext, Exception e)
    {
        // The exception itself is never written out, so nothing internal leaks into the response.
        var problem = e is ValidationException validationException
            ? (IResult)TypedResults.ValidationProblem(ToErrors(validationException))
            : TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError);

        return problem.ExecuteAsync(httpContext);
    }

    /// <summary>
    /// Groups the failures by property, named as the property appears in the JSON request body.
    /// </summary>
    private static Dictionary<string, string[]> ToErrors(ValidationException validationException)
    {
        return validationException.Errors
            .GroupBy(failure => JsonNamingPolicy.CamelCase.ConvertName(failure.PropertyName))
            .ToDictionary(group => group.Key, group => group.Select(failure => failure.ErrorMessage).ToArray());
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static void UseCustomExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
