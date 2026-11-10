namespace Rentar.Modules.Identity.Presentation.Endpoints.V2;

/// <summary>
/// Body for creating or replacing a user; the id comes from the URL, never from the body. The password is
/// sent in plain text and only its salted hash is stored.
/// </summary>
public sealed record UserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    int FindeksRating);
