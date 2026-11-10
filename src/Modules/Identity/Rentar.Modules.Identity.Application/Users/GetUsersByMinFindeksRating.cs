using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Application.UseCases;
using System.Collections.Generic;

namespace Rentar.Modules.Identity.Application.Users;

/// <summary>The users whose Findeks rating is at least <see cref="MinFindeksRating"/>.</summary>
public sealed record GetUsersByMinFindeksRating(int MinFindeksRating);

public sealed class GetUsersByMinFindeksRatingHandler(IUserDal userDal) : IQueryHandler<GetUsersByMinFindeksRating, List<User>>
{
    public List<User> Handle(GetUsersByMinFindeksRating query)
    {
        return userDal.GetAll(u => u.FindeksRating >= query.MinFindeksRating);
    }
}
