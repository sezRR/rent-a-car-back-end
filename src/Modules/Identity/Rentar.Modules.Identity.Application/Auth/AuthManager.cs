using Rentar.Modules.Identity.Application.Security;
using Rentar.Modules.Identity.Application.Users;
using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Kernel.Results;

namespace Rentar.Modules.Identity.Application.Auth;

public class AuthManager(IUserService userService, ITokenHelper tokenHelper) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly ITokenHelper _tokenHelper = tokenHelper;

    public IDataResult<AccessToken> CreateAccessToken(User user)
    {
        var claims = _userService.GetClaims(user);
        var accessToken = _tokenHelper.CreateToken(user, claims);
        return new SuccessDataResult<AccessToken>(accessToken, Messages.AccessTokenCreated);
    }

    public IDataResult<User> Login(UserForLoginDto userForLoginDto)
    {
        var userToCheck = _userService.GetByMail(userForLoginDto.Email);
        if (userToCheck == null)
        {
            return new ErrorDataResult<User>(Messages.UserNotFound);
        }

        if (!HashingHelper.VerifyPasswordHash(userForLoginDto.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
        {
            return new ErrorDataResult<User>(Messages.PasswordError);
        }
        return new SuccessDataResult<User>(userToCheck, Messages.SuccessfulLogin);
    }

    public IDataResult<User> Register(UserForRegisterDto userForRegisterDto, string password)
    {
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
        return new SuccessDataResult<User>(user, Messages.UserRegistered);
    }

    public IResult UserExists(string email)
    {
        if (_userService.GetByMail(email) != null)
        {
            return new ErrorResult(Messages.UserAlreadyExists);
        }
        return new SuccessResult();
    }
}
