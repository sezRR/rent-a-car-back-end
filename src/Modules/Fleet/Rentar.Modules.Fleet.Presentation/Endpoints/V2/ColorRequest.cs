namespace Rentar.Modules.Fleet.Presentation.Endpoints.V2;

/// <summary>
/// Body for creating or replacing a color; the id comes from the URL, never from the body.
/// </summary>
public sealed record ColorRequest(string ColorName);
