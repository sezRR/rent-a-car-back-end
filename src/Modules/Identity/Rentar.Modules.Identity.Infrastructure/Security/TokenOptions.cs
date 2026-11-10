namespace Rentar.Modules.Identity.Infrastructure.Security;

public class TokenOptions
{
    public const string SectionName = "TokenOptions";

    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int AccessTokenExpiration { get; set; }
    public string SecurityKey { get; set; }
}
