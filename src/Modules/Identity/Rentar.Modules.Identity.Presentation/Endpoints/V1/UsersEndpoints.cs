using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Identity.Application.Users;
using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Presentation.Endpoints;

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

    private static IResult GetAll(IUserService userService)
    {
        var result = userService.GetAll();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult GetByMail(IUserService userService, string email)
    {
        var result = userService.GetUserByMail(email);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult GetUserFindeksRating(IUserService userService, int findeksRating)
    {
        var result = userService.GetUserFindeksRating(findeksRating);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult GetUserById(IUserService userService, int id)
    {
        var result = userService.GetUserById(id);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult Add(IUserService userService, User user)
    {
        var result = userService.Add(user);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static IResult Update(IUserService userService, UserForUpdateDto userForUpdateDto)
    {
        var result = userService.Update(userForUpdateDto);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
