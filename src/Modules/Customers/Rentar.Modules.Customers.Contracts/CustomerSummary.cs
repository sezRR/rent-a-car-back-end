namespace Rentar.Modules.Customers.Contracts;

/// <param name="UserId">The Identity user this customer account belongs to.</param>
public sealed record CustomerSummary(int CustomerId, int UserId, string CompanyName);
