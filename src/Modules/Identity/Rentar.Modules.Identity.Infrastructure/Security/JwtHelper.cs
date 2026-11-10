using Microsoft.Extensions.Options;
using Rentar.Modules.Identity.Application.Auth;
using Rentar.Modules.Identity.Domain;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Rentar.Modules.Identity.Infrastructure.Security;

public class JwtHelper(IOptions<TokenOptions> tokenOptions, TimeProvider timeProvider) : ITokenHelper
{
    private readonly TokenOptions _tokenOptions = tokenOptions.Value;
    private readonly TimeProvider _timeProvider = timeProvider;

    public AccessToken CreateToken(User user, List<OperationClaim> operationClaims)
    {
        // Worked out per token: the helper is a singleton, so a value fixed at construction would expire every token issued later.
        var expiration = _timeProvider.GetLocalNow().DateTime.AddMinutes(_tokenOptions.AccessTokenExpiration);

        var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
        var signingCredentials = SecurityKeyHelper.CreateSigningCredentials(securityKey);
        var jwt = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            expires: expiration,
            notBefore: _timeProvider.GetUtcNow().UtcDateTime,
            claims: SetClaims(user, operationClaims),
            signingCredentials: signingCredentials);

        return new AccessToken
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwt),
            Expiration = expiration
        };
    }

    private static IEnumerable<Claim> SetClaims(User user, List<OperationClaim> operationClaims)
    {
        var claims = new List<Claim>();
        claims.AddNameIdentifier(user.Id.ToString());
        claims.AddEmail(user.Email);
        claims.AddName($"{user.FirstName} {user.LastName}");
        claims.AddRoles(operationClaims.Select(c => c.Name).ToArray());
        return claims;
    }
}
