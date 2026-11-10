namespace Rentar.Shared.Kernel.Exceptions;

/// <summary>
/// The email and password do not match a user.
/// </summary>
public class InvalidCredentialsException(string message) : RentarException(message);
