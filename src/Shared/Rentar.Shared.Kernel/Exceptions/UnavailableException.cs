namespace Rentar.Shared.Kernel.Exceptions;

/// <summary>
/// The operation is closed for now, e.g. during the 22:00 maintenance hour.
/// </summary>
public class UnavailableException(string message) : RentarException(message);
