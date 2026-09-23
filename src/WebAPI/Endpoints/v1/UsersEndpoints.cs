using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WebAPI.Common;
using User = Core.Entities.Concrete.User;

namespace WebAPI.Endpoints.v1;

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

    private IResult GetAll(IUserService userService)
    {
        var result = userService.GetAll();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult GetByMail(IUserService userService, string email)
    {
        var result = userService.GetUserByMail(email);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult GetUserFindeksRating(IUserService userService, int findeksRating)
    {
        var result = userService.GetUserFindeksRating(findeksRating);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult GetUserById(IUserService userService, int id)
    {
        var result = userService.GetUserById(id);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Add(IUserService userService, User user)
    {
        var result = userService.Add(user);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult Update(IUserService userService, UserForUpdateDto userForUpdateDto)
    {
        var result = userService.Update(userForUpdateDto);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
