#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Identity.Application.Auth;
using Rentar.Modules.Identity.Application.Users;
using Rentar.Shared.Presentation.Endpoints;
using System.Collections.Generic;

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
    /// The full list is closed during the 22:00 maintenance hour.
    /// </summary>
    private static Ok<List<UserResponse>> GetAll(IUserService userService, string? email, int? minFindeksRating)
    {
        if (email is not null)
        {
            var user = userService.GetByMail(email);
            var matches = new List<UserResponse>();

            if (user is not null && user.FindeksRating >= (minFindeksRating ?? int.MinValue))
            {
                matches.Add(UserResponse.From(user));
            }

            return TypedResults.Ok(matches);
        }

        var users = minFindeksRating is { } rating
            ? userService.GetUserFindeksRating(rating)
            : userService.GetAll();

        return TypedResults.Ok(users.ConvertAll(UserResponse.From));
    }

    private static Results<Ok<UserResponse>, ProblemHttpResult> GetById(IUserService userService, int id)
    {
        var user = userService.GetById(id);
        return user is null ? RestResults.NotFound("User", id) : TypedResults.Ok(UserResponse.From(user));
    }

    /// <summary>
    /// Creates an account; an email that is already registered answers 409. Log in through
    /// <c>POST /auth/tokens</c> to get an access token for it.
    /// </summary>
    private static Created<UserResponse> Create(IAuthService authService, HttpRequest request, UserRequest body)
    {
        var registration = new UserForRegisterDto
        {
            FirstName = body.FirstName,
            LastName = body.LastName,
            Email = body.Email,
            Password = body.Password,
            FindeksRating = body.FindeksRating
        };

        var user = authService.Register(registration, body.Password);
        return RestResults.Created(request, user.Id, UserResponse.From(user));
    }

    private static Results<NoContent, ProblemHttpResult> Update(IUserService userService, int id, UserRequest body)
    {
        if (userService.GetById(id) is null)
        {
            return RestResults.NotFound("User", id);
        }

        userService.Update(new UserForUpdateDto
        {
            Id = id,
            FirstName = body.FirstName,
            LastName = body.LastName,
            Email = body.Email,
            Password = body.Password,
            FindeksRating = body.FindeksRating
        });

        return TypedResults.NoContent();
    }
}
