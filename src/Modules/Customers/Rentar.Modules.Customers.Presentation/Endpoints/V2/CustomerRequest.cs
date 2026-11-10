namespace Rentar.Modules.Customers.Presentation.Endpoints.V2;

/// <summary>
/// Body for creating a customer; <see cref="UserId"/> is the Identity user the customer belongs to.
/// </summary>
public sealed record CustomerRequest(int UserId, string CompanyName);
