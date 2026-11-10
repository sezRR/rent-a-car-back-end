using Rentar.Modules.Findeks.Domain;
using System;

namespace Rentar.Modules.Findeks.Application;

public class FindeksManager : IFindeksService
{
    public int CalculateFindeksRating()
    {
        // No credit bureau is connected, so each factor is simulated up to its maximum.
        var report = new FindeksReport
        {
            CreditProductRepayments = Random.Shared.Next(666), // 665
            CurrentAccountsAndDebts = Random.Shared.Next(666), // 665
            LoanUsageIntensity = Random.Shared.Next(191),      // 190
            NewLoansTaken = Random.Shared.Next(210),           // 209
            OtherFactors = Random.Shared.Next(172),            // 171
        };

        var findeksRating = report.CreditProductRepayments
            + report.CurrentAccountsAndDebts
            + report.LoanUsageIntensity
            + report.NewLoansTaken
            + report.OtherFactors;

        report.MinimumFindeksRating = findeksRating;

        return findeksRating;
    }
}
