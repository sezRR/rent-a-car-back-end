namespace Rentar.Shared.Kernel.Exceptions;

/// <summary>
/// The request clashes with what is already stored, e.g. an email that is already registered.
/// </summary>
public class ConflictException(string message) : RentarException(message);
