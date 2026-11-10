namespace Rentar.Modules.Fleet.Presentation.Endpoints.V2;

/// <summary>
/// Body for creating or replacing a brand; the id comes from the URL, never from the body.
/// </summary>
public sealed record BrandRequest(string BrandName);
