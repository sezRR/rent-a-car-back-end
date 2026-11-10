using System;

namespace Rentar.Modules.Identity.Application.Auth;

public class AccessToken
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}
