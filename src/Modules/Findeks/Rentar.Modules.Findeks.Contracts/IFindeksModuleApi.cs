namespace Rentar.Modules.Findeks.Contracts;

/// <summary>
/// What other modules may ask the Findeks module.
/// </summary>
public interface IFindeksModuleApi
{
    /// <summary>A Findeks credit rating between 0 and 1900.</summary>
    int CalculateFindeksRating();
}
