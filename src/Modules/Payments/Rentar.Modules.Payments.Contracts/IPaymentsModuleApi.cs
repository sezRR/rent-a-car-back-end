namespace Rentar.Modules.Payments.Contracts;

/// <summary>
/// What other modules may ask the Payments module.
/// </summary>
public interface IPaymentsModuleApi
{
    /// <summary>Takes a payment; returns whether it went through.</summary>
    bool Pay();
}
