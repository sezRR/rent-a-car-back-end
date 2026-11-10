using System;

namespace Rentar.Shared.Kernel.Exceptions;

/// <summary>
/// A request the application refused for a known reason. The message is meant for the caller, so it never holds
/// internal details. Each subclass says why, and the API turns that into a status code.
/// </summary>
public abstract class RentarException(string message) : Exception(message);
