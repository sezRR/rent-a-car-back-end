using Rentar.Modules.Identity.Application.Users;
using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Identity.Application.Auth;

/// <summary>Issues an access token carrying the user's operation claims.</summary>
public sealed record CreateAccessToken(User User);

public sealed class CreateAccessTokenHandler(IUserDal userDal, ITokenHelper tokenHelper) : ICommandHandler<CreateAccessToken, AccessToken>
{
    public AccessToken Handle(CreateAccessToken command)
    {
        return tokenHelper.CreateToken(command.User, userDal.GetClaims(command.User));
    }
}
