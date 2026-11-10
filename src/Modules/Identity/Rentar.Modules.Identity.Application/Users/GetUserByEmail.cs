using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Identity.Application.Users;

/// <summary>Answers the user, or <c>null</c> when no user has that email.</summary>
public sealed record GetUserByEmail(string Email);

public sealed class GetUserByEmailHandler(IUserDal userDal) : IQueryHandler<GetUserByEmail, User>
{
    public User Handle(GetUserByEmail query)
    {
        return userDal.Get(u => u.Email == query.Email);
    }
}
