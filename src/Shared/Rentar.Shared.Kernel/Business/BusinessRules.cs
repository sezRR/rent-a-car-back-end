using Rentar.Shared.Kernel.Results;

namespace Rentar.Shared.Kernel.Business;

public static class BusinessRules
{
    /// <summary>
    /// Returns the first failed result, or <c>null</c> when every rule passed.
    /// </summary>
    public static IResult Run(params IResult[] logics)
    {
        foreach (var logic in logics)
        {
            if (!logic.Success)
            {
                return logic;
            }
        }

        return null;
    }
}
