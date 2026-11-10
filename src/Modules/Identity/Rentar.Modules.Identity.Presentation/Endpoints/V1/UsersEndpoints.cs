using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Identity.Application;
using Rentar.Modules.Identity.Application.Users;
using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Presentation.Endpoints;
using System.Collections.Generic;

namespace Rentar.Modules.Identity.Presentation.Endpoints.V1;

public class UsersEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Users", "users", 1.0);

        group.MapGet("/getall", GetAll);
        group.MapGet("/getbymail", GetByMail);
        group.MapGet("/getuserfindeksrating", GetUserFindeksRating);
        group.MapGet("/getuserbyid", GetUserById);
        group.MapPost("/add", Add);
        group.MapPost("/update", Update);
    }

    private static Ok<V1Response<List<User>>> GetAll(IUserService userService)
    {
        return V1Results.Ok(userService.GetAll(), Messages.UsersListed);
    }

    private static Ok<V1Response<User>> GetByMail(IUserService userService, string email)
    {
        return V1Results.Ok(userService.GetByMail(email), null);
    }

    private static Ok<V1Response<List<User>>> GetUserFindeksRating(IUserService userService, int findeksRating)
    {
        return V1Results.Ok(userService.GetUserFindeksRating(findeksRating), null);
    }

    private static Ok<V1Response<User>> GetUserById(IUserService userService, int id)
    {
        return V1Results.Ok(userService.GetById(id), null);
    }

    private static Ok<V1Response> Add(IUserService userService, User user)
    {
        userService.Add(user);
        return V1Results.Ok(Messages.UserAdded);
    }

    private static Ok<V1Response<User>> Update(IUserService userService, UserForUpdateDto userForUpdateDto)
    {
        return V1Results.Ok(userService.Update(userForUpdateDto), Messages.UserUpdated);
    }
}
