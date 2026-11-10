using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Identity.Application;
using Rentar.Modules.Identity.Application.Users;
using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Application.UseCases;
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

    private static Ok<V1Response<List<User>>> GetAll(IQueryHandler<GetUsers, List<User>> getUsers)
    {
        return V1Results.Ok(getUsers.Handle(new GetUsers()), Messages.UsersListed);
    }

    private static Ok<V1Response<User>> GetByMail(IQueryHandler<GetUserByEmail, User> getUserByEmail, string email)
    {
        return V1Results.Ok(getUserByEmail.Handle(new GetUserByEmail(email)), null);
    }

    private static Ok<V1Response<List<User>>> GetUserFindeksRating(
        IQueryHandler<GetUsersByMinFindeksRating, List<User>> getUsersByMinFindeksRating,
        int findeksRating)
    {
        return V1Results.Ok(getUsersByMinFindeksRating.Handle(new GetUsersByMinFindeksRating(findeksRating)), null);
    }

    private static Ok<V1Response<User>> GetUserById(IQueryHandler<GetUserById, User> getUserById, int id)
    {
        return V1Results.Ok(getUserById.Handle(new GetUserById(id)), null);
    }

    private static Ok<V1Response> Add(ICommandHandler<AddUser> addUser, AddUser command)
    {
        addUser.Handle(command);
        return V1Results.Ok(Messages.UserAdded);
    }

    private static Ok<V1Response<User>> Update(ICommandHandler<UpdateUser, User> updateUser, UpdateUser command)
    {
        return V1Results.Ok(updateUser.Handle(command), Messages.UserUpdated);
    }
}
