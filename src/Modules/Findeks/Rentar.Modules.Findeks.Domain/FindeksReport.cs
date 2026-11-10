using Rentar.Shared.Kernel.Entities;

namespace Rentar.Modules.Findeks.Domain;

/// <summary>
/// The factors a Findeks credit rating is made of. Their maximums add up to 1900, the highest rating.
/// </summary>
public class FindeksReport : IEntity
{
    public int CreditProductRepayments { get; set; }
    public int CurrentAccountsAndDebts { get; set; }
    public int LoanUsageIntensity { get; set; }
    public int NewLoansTaken { get; set; }
    public int OtherFactors { get; set; }
    public int MinimumFindeksRating { get; set; }
}
