using Rentar.Modules.Identity.Application.Security;
using Rentar.Modules.Identity.Application.Users;
using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Kernel.Exceptions;

namespace Rentar.Modules.Identity.Application.Auth;

/// <summary>
/// Answers the user the email and password belong to. Throws <see cref="InvalidCredentialsException"/> when no user
/// has that email or the password is wrong.
/// </summary>
public sealed record VerifyCredentials(string Email, string Password);

public sealed class VerifyCredentialsHandler(IUserDal userDal) : IQueryHandler<VerifyCredentials, User>
{
    public User Handle(VerifyCredentials query)
    {
        var user = userDal.Get(u => u.Email == query.Email)
            ?? throw new InvalidCredentialsException(Messages.UserNotFound);

        if (!HashingHelper.VerifyPasswordHash(query.Password, user.PasswordHash, user.PasswordSalt))
        {
            throw new InvalidCredentialsException(Messages.PasswordError);
        }

        return user;
    }
}
