namespace Rentar.Shared.Presentation.Endpoints;

/// <summary>
/// The <c>{ success, message }</c> body every v1 endpoint has always answered with. Only v1 uses it: v2 returns the
/// resource itself and problem details on failure.
/// </summary>
public record V1Response(bool Success, string Message);

/// <summary>
/// The <c>{ data, success, message }</c> body of the v1 endpoints that return data.
/// </summary>
public record V1Response<T>(T Data, bool Success, string Message);
