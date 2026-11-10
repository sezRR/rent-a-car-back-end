namespace Rentar.Shared.Kernel.Exceptions;

/// <summary>
/// A business rule rejected the request, e.g. a car already has the most images it may have.
/// </summary>
public class BusinessRuleException(string message) : RentarException(message);
