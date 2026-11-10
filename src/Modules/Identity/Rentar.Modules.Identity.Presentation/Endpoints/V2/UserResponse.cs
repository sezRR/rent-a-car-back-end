using Rentar.Modules.Identity.Domain;

namespace Rentar.Modules.Identity.Presentation.Endpoints.V2;

/// <summary>
/// A user as v2 returns it. The password hash and salt are never part of a response.
/// </summary>
public sealed record UserResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    bool Status,
    int FindeksRating)
{
    internal static UserResponse From(User user)
    {
        return new UserResponse(user.Id, user.FirstName, user.LastName, user.Email, user.Status, user.FindeksRating);
    }
}
