using Rentar.Modules.Identity.Application.Security;
using Rentar.Modules.Identity.Application.Users;
using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Kernel.Exceptions;

namespace Rentar.Modules.Identity.Application.Auth;

public class AuthManager(IUserService userService, ITokenHelper tokenHelper) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly ITokenHelper _tokenHelper = tokenHelper;

    public AccessToken CreateAccessToken(User user)
    {
        var claims = _userService.GetClaims(user);
        return _tokenHelper.CreateToken(user, claims);
    }

    public User Login(UserForLoginDto userForLoginDto)
    {
        var userToCheck = _userService.GetByMail(userForLoginDto.Email)
            ?? throw new InvalidCredentialsException(Messages.UserNotFound);

        if (!HashingHelper.VerifyPasswordHash(userForLoginDto.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
        {
            throw new InvalidCredentialsException(Messages.PasswordError);
        }

        return userToCheck;
    }

    public User Register(UserForRegisterDto userForRegisterDto, string password)
    {
        if (_userService.GetByMail(userForRegisterDto.Email) != null)
        {
            throw new ConflictException(Messages.UserAlreadyExists);
        }

        HashingHelper.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

        var user = new User
        {
            FirstName = userForRegisterDto.FirstName,
            LastName = userForRegisterDto.LastName,
            Email = userForRegisterDto.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Status = true,
            FindeksRating = userForRegisterDto.FindeksRating,
        };

        _userService.Add(user);
        return user;
    }
}
