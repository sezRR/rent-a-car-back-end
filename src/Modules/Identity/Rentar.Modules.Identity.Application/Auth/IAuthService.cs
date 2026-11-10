using Rentar.Modules.Identity.Domain;

namespace Rentar.Modules.Identity.Application.Auth;

public interface IAuthService
{
    /// <summary>Creates the user with a salted hash of <paramref name="password"/>.</summary>
    /// <exception cref="Rentar.Shared.Kernel.Exceptions.ConflictException">A user with that email already exists.</exception>
    User Register(UserForRegisterDto userForRegisterDto, string password);

    /// <returns>The user the credentials belong to.</returns>
    /// <exception cref="Rentar.Shared.Kernel.Exceptions.InvalidCredentialsException">No user has that email, or the password is wrong.</exception>
    User Login(UserForLoginDto userForLoginDto);

    AccessToken CreateAccessToken(User user);
}
