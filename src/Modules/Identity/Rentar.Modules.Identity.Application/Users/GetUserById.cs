using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Identity.Application.Users;

/// <summary>Answers the user, or <c>null</c> when there is none with that id.</summary>
public sealed record GetUserById(int Id);

public sealed class GetUserByIdHandler(IUserDal userDal) : IQueryHandler<GetUserById, User>
{
    public User Handle(GetUserById query)
    {
        return userDal.Get(u => u.Id == query.Id);
    }
}
