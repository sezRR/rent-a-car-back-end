using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WebAPI.Common;

namespace WebAPI.Endpoints.v1;

public class AuthEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Auth", "auth", 1.0);

        group.MapPost("/login", Login);
        group.MapPost("/checkpassword", CheckPassword);
        group.MapPost("/register", Register);
    }

    private IResult Login(IAuthService authService, UserForLoginDto userForLoginDto)
    {
        var userToLogin = authService.Login(userForLoginDto);
        if (!userToLogin.Success)
        {
            return Results.BadRequest(userToLogin.Message);
        }

        var result = authService.CreateAccessToken(userToLogin.Data);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private IResult CheckPassword(IAuthService authService, UserForLoginDto userForLoginDto)
    {
        var userToLogin = authService.Login(userForLoginDto);
        return userToLogin.Success ? Results.Ok(userToLogin) : Results.BadRequest(userToLogin.Message);
    }

    private IResult Register(IAuthService authService, UserForRegisterDto userForRegisterDto)
    {
        var userExists = authService.UserExists(userForRegisterDto.Email);
        if (!userExists.Success)
        {
            return Results.BadRequest(userExists);
        }

        var registerResult = authService.Register(userForRegisterDto, userForRegisterDto.Password);
        var result = authService.CreateAccessToken(registerResult.Data);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
