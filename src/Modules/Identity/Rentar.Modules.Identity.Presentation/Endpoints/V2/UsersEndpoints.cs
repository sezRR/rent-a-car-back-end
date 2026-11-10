#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Identity.Application;
using Rentar.Modules.Identity.Application.Auth;
using Rentar.Modules.Identity.Application.Users;
using Rentar.Shared.Presentation.Endpoints;
using System.Collections.Generic;
using KernelResult = Rentar.Shared.Kernel.Results.IResult;

namespace Rentar.Modules.Identity.Presentation.Endpoints.V2;

public class UsersEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Users", "users", 2.0);

        group.MapGet("/", GetAll)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);
        group.MapGet("/{id:int}", GetById)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPost("/", Create)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapPut("/{id:int}", Update)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Lists users, optionally only the one with an email and/or those with at least a Findeks rating.
    /// </summary>
    private static Results<Ok<List<UserResponse>>, ProblemHttpResult> GetAll(
        IUserService userService,
        string? email,
        int? minFindeksRating)
    {
        if (email is not null)
        {
            var user = userService.GetUserByMail(email).Data;
            var matches = new List<UserResponse>();

            if (user is not null && user.FindeksRating >= (minFindeksRating ?? int.MinValue))
            {
                matches.Add(UserResponse.From(user));
            }

            return TypedResults.Ok(matches);
        }

        var result = minFindeksRating is { } rating
            ? userService.GetUserFindeksRating(rating)
            : userService.GetAll();

        return result.Success ? TypedResults.Ok(result.Data.ConvertAll(UserResponse.From)) : Problem(result);
    }

    private static Results<Ok<UserResponse>, ProblemHttpResult> GetById(IUserService userService, int id)
    {
        var user = userService.GetUserById(id).Data;
        return user is null ? RestResults.NotFound("User", id) : TypedResults.Ok(UserResponse.From(user));
    }

    /// <summary>
    /// Creates an account. Log in through <c>POST /auth/tokens</c> to get an access token for it.
    /// </summary>
    private static Results<Created<UserResponse>, ProblemHttpResult> Create(
        IAuthService authService,
        HttpRequest request,
        UserRequest body)
    {
        var userExists = authService.UserExists(body.Email);
        if (!userExists.Success)
        {
            return RestResults.Problem(userExists, StatusCodes.Status409Conflict);
        }

        var registration = new UserForRegisterDto
        {
            FirstName = body.FirstName,
            LastName = body.LastName,
            Email = body.Email,
            Password = body.Password,
            FindeksRating = body.FindeksRating
        };

        var result = authService.Register(registration, body.Password);
        return result.Success
            ? RestResults.Created(request, result.Data.Id, UserResponse.From(result.Data))
            : RestResults.Problem(result);
    }

    private static Results<NoContent, ProblemHttpResult> Update(IUserService userService, int id, UserRequest body)
    {
        if (userService.GetUserById(id).Data is null)
        {
            return RestResults.NotFound("User", id);
        }

        var result = userService.Update(new UserForUpdateDto
        {
            Id = id,
            FirstName = body.FirstName,
            LastName = body.LastName,
            Email = body.Email,
            Password = body.Password,
            FindeksRating = body.FindeksRating
        });

        return result.Success ? TypedResults.NoContent() : RestResults.Problem(result);
    }

    /// <summary>
    /// The user list is closed during the 22:00 maintenance hour, which is a temporary outage rather than a bad request.
    /// </summary>
    private static ProblemHttpResult Problem(KernelResult result)
    {
        return RestResults.Problem(result, result.Message == Messages.MaintenanceTime
            ? StatusCodes.Status503ServiceUnavailable
            : StatusCodes.Status400BadRequest);
    }
}
