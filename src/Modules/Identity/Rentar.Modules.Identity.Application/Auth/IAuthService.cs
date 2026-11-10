using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Kernel.Results;

namespace Rentar.Modules.Identity.Application.Auth;

public interface IAuthService
{
    IDataResult<User> Register(UserForRegisterDto userForRegisterDto, string password);
    IDataResult<User> Login(UserForLoginDto userForLoginDto);
    IDataResult<AccessToken> CreateAccessToken(User user);

    IResult UserExists(string email);
}
