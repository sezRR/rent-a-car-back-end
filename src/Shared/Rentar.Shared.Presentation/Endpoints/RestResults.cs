using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Rentar.Shared.Presentation.Endpoints;

/// <summary>
/// Responses for the RESTful endpoints (v2 onwards). Resources are returned as they are, without the v1
/// envelope; failures thrown by the services become RFC 9457 problem details in <c>ApiExceptionHandler</c>.
/// </summary>
public static class RestResults
{
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
