namespace Rentar.Modules.Fleet.Presentation.Endpoints.V2;

/// <summary>
/// Body for creating or replacing a car; the id comes from the URL, never from the body.
/// </summary>
public sealed record CarRequest(
    int BrandId,
    int ColorId,
    int ModelYear,
    decimal DailyPrice,
    string Description,
    int MinimumFindeksRating);
