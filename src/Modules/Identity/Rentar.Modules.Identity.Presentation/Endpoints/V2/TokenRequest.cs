namespace Rentar.Modules.Identity.Presentation.Endpoints.V2;

/// <summary>
/// The credentials an access token is issued for.
/// </summary>
public sealed record TokenRequest(string Email, string Password);
