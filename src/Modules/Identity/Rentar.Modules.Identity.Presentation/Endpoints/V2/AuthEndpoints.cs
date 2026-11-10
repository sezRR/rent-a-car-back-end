#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Identity.Application.Auth;
using Rentar.Shared.Presentation.Endpoints;

namespace Rentar.Modules.Identity.Presentation.Endpoints.V2;

/// <summary>
/// Logging in creates an access token. Accounts are created through <c>POST /users</c>.
/// </summary>
public class AuthEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Auth", "auth", 2.0);

        group.MapPost("/tokens", CreateToken)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static Results<Ok<AccessToken>, ProblemHttpResult> CreateToken(IAuthService authService, TokenRequest body)
    {
        var login = authService.Login(new UserForLoginDto { Email = body.Email, Password = body.Password });
        if (!login.Success)
        {
            // One answer for an unknown email and a wrong password, so the response does not reveal which accounts exist.
            return TypedResults.Problem(detail: "Email or password is wrong.", statusCode: StatusCodes.Status401Unauthorized);
        }

        var result = authService.CreateAccessToken(login.Data);
        return result.Success ? TypedResults.Ok(result.Data) : RestResults.Problem(result);
    }
}
