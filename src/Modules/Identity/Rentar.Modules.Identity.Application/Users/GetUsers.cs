using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Kernel.Exceptions;
using System;
using System.Collections.Generic;

namespace Rentar.Modules.Identity.Application.Users;

/// <summary>Every user; throws <see cref="UnavailableException"/> during the 22:00 maintenance hour.</summary>
public sealed record GetUsers;

public sealed class GetUsersHandler(IUserDal userDal, TimeProvider timeProvider) : IQueryHandler<GetUsers, List<User>>
{
    public List<User> Handle(GetUsers query)
    {
        if (timeProvider.GetLocalNow().Hour == 22)
        {
            throw new UnavailableException(Messages.MaintenanceTime);
        }

        return userDal.GetAll();
    }
}
