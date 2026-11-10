using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rentar.Shared.Kernel.Exceptions;
using Rentar.Shared.Presentation.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rentar.Shared.Presentation.Errors;

/// <summary>
/// Turns an exception thrown while handling a request into its response. From v2 on that is an RFC 9457 problem;
/// v1 keeps the bodies it has always sent. Unexpected exceptions never reveal anything about themselves.
/// </summary>
public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    private static readonly ApiVersion V1 = new(1, 0);

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Refused requests are expected; anything else is a bug, and the response says nothing about it, so it is logged here.
        if (exception is not (RentarException or ValidationException))
        {
            logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        }

        var response = IsV1(httpContext) ? ToV1Response(exception) : ToProblem(exception);
        await response.ExecuteAsync(httpContext);
        return true;
    }

    /// <summary>
    /// The exception handler clears the route values before it calls handlers, so the version is read from the
    /// endpoint that failed rather than from the request.
    /// </summary>
    private static bool IsV1(HttpContext httpContext)
    {
        var endpoint = httpContext.Features.Get<IExceptionHandlerFeature>()?.Endpoint;
        return endpoint?.Metadata.GetMetadata<ApiVersionMetadata>()?.IsMappedTo(V1) == true;
    }

    private static IResult ToProblem(Exception exception)
    {
        var problem = exception switch
        {
            ValidationException validation => new HttpValidationProblemDetails(ToErrors(validation))
            {
                Status = StatusCodes.Status400BadRequest
            },
            // One answer for an unknown email and a wrong password, so the response does not reveal which accounts exist.
            InvalidCredentialsException => Problem(StatusCodes.Status401Unauthorized, "Email or password is wrong."),
            NotFoundException notFound => Problem(StatusCodes.Status404NotFound, notFound.Message),
            ConflictException conflict => Problem(StatusCodes.Status409Conflict, conflict.Message),
            UnavailableException unavailable => Problem(StatusCodes.Status503ServiceUnavailable, unavailable.Message),
            RentarException refused => Problem(StatusCodes.Status400BadRequest, refused.Message),
            _ => Problem(StatusCodes.Status500InternalServerError, null)
        };

        return TypedResults.Problem(problem);
    }

    private static IResult ToV1Response(Exception exception)
    {
        return exception switch
        {
            ValidationException validation => Json(StatusCodes.Status400BadRequest, new ValidationErrorDetails
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = validation.Message,
                Errors = validation.Errors
            }),
            // The maintenance hour only ever closed queries that return data, and v1 sent their failures with "data": null.
            UnavailableException unavailable => TypedResults.BadRequest(new V1Response<object>(null, false, unavailable.Message)),
            RentarException refused => V1Results.Fail(refused.Message),
            _ => Json(StatusCodes.Status500InternalServerError, new ErrorDetails
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Internal Server Error"
            })
        };
    }

    private static ProblemDetails Problem(int status, string detail)
    {
        return new ProblemDetails { Status = status, Detail = detail };
    }

    /// <summary>
    /// v1's error details are written with Newtonsoft, so they keep the PascalCase names v1 clients read.
    /// </summary>
    private static IResult Json(int status, ErrorDetails details)
    {
        return TypedResults.Text(details.ToString(), "application/json", statusCode: status);
    }

    /// <summary>
    /// Groups the failures by property, named as the property appears in the JSON request body.
    /// </summary>
    private static Dictionary<string, string[]> ToErrors(ValidationException validation)
    {
        return validation.Errors
            .GroupBy(failure => JsonNamingPolicy.CamelCase.ConvertName(failure.PropertyName))
            .ToDictionary(group => group.Key, group => group.Select(failure => failure.ErrorMessage).ToArray());
    }
}
