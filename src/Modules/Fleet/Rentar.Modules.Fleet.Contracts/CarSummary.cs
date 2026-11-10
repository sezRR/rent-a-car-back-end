namespace Rentar.Modules.Fleet.Contracts;

public sealed record CarSummary(
    int CarId,
    string BrandName,
    string ColorName,
    int ModelYear,
    decimal DailyPrice,
    string Description);
