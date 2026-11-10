using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using KernelResult = Rentar.Shared.Kernel.Results.IResult;

namespace Rentar.Shared.Presentation.Endpoints;

/// <summary>
/// Responses for the RESTful endpoints (v2 onwards). Resources are returned as they are, without the
/// v1 result envelope, and failures are returned as RFC 9457 problem details.
/// </summary>
public static class RestResults
{
    /// <summary>
    /// Turns a failed service result into a problem whose detail is the service's message.
    /// </summary>
    public static ProblemHttpResult Problem(KernelResult result, int statusCode = StatusCodes.Status400BadRequest)
    {
        return TypedResults.Problem(detail: result.Message, statusCode: statusCode);
    }

    public static ProblemHttpResult NotFound(string resource, int id)
    {
        return TypedResults.Problem(detail: $"{resource} {id} was not found.", statusCode: StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Answers 201 with the new resource's URL, which is the collection it was posted to followed by its id.
    /// </summary>
    public static Created<T> Created<T>(HttpRequest request, int id, T resource)
    {
        var collection = $"{request.PathBase}{request.Path}".TrimEnd('/');
        return TypedResults.Created($"{collection}/{id}", resource);
    }
}
