using Rentar.Modules.Identity.Application.Security;
using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Identity.Application.Users;

/// <summary>Replaces the user, storing a fresh salted hash of <see cref="Password"/>, and answers the stored user.</summary>
public sealed record UpdateUser(int Id, string FirstName, string LastName, string Email, string Password, int FindeksRating);

public sealed class UpdateUserHandler(IUserDal userDal) : ICommandHandler<UpdateUser, User>
{
    public User Handle(UpdateUser command)
    {
        HashingHelper.CreatePasswordHash(command.Password, out var passwordHash, out var passwordSalt);

        var user = new User
        {
            Id = command.Id,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Status = true,
            FindeksRating = command.FindeksRating,
        };

        userDal.Update(user);
        return user;
    }
}
