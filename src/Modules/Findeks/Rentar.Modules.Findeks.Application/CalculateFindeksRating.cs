using Rentar.Modules.Findeks.Domain;
using Rentar.Shared.Application.UseCases;
using System;

namespace Rentar.Modules.Findeks.Application;

/// <summary>A Findeks credit rating. No credit bureau is connected, so the rating is simulated.</summary>
public sealed record CalculateFindeksRating;

public sealed class CalculateFindeksRatingHandler : IQueryHandler<CalculateFindeksRating, int>
{
    public int Handle(CalculateFindeksRating query)
    {
        // Each factor is simulated up to its maximum.
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
