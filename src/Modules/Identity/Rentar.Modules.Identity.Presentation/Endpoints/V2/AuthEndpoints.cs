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

    /// <summary>
    /// Bad credentials answer 401 with the same detail whatever was wrong, so the response does not reveal which
    /// accounts exist.
    /// </summary>
    private static Ok<AccessToken> CreateToken(IAuthService authService, TokenRequest body)
    {
        var user = authService.Login(new UserForLoginDto { Email = body.Email, Password = body.Password });
        return TypedResults.Ok(authService.CreateAccessToken(user));
    }
}
