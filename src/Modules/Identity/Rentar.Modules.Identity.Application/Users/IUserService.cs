using Rentar.Modules.Identity.Domain;
using System.Collections.Generic;

namespace Rentar.Modules.Identity.Application.Users;

public interface IUserService
{
    /// <exception cref="Rentar.Shared.Kernel.Exceptions.UnavailableException">It is the 22:00 maintenance hour.</exception>
    List<User> GetAll();

    List<OperationClaim> GetClaims(User user);

    /// <returns>The user, or <c>null</c> when no user has that email.</returns>
    User GetByMail(string email);

    /// <returns>The user, or <c>null</c> when there is none with that id.</returns>
    User GetById(int id);

    void Add(User user);

    /// <summary>Replaces the user, hashing the new password, and returns the stored user.</summary>
    User Update(UserForUpdateDto userForUpdateDto);

    /// <exception cref="Rentar.Shared.Kernel.Exceptions.BusinessRuleException">The first name is shorter than two characters.</exception>
    void Delete(User user);

    /// <summary>The users whose Findeks rating is at least <paramref name="findeksRating"/>.</summary>
    List<User> GetUserFindeksRating(int findeksRating);
}
