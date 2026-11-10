namespace Rentar.Shared.Kernel.Exceptions;

/// <summary>
/// The record the request refers to does not exist.
/// </summary>
public class NotFoundException(string message) : RentarException(message);
