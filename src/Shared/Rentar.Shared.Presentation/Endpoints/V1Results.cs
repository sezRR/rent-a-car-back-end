using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Rentar.Shared.Presentation.Endpoints;

/// <summary>
/// Responses in v1's original shape. Each endpoint passes the exact message v1 has always sent, <c>null</c> included.
/// </summary>
public static class V1Results
{
    public static Ok<V1Response> Ok(string message)
    {
        return TypedResults.Ok(new V1Response(true, message));
    }

    public static Ok<V1Response<T>> Ok<T>(T data, string message)
    {
        return TypedResults.Ok(new V1Response<T>(data, true, message));
    }

    public static BadRequest<V1Response> Fail(string message)
    {
        return TypedResults.BadRequest(new V1Response(false, message));
    }
}
