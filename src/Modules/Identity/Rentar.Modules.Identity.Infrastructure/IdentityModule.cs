using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rentar.Modules.Identity.Application;
using Rentar.Modules.Identity.Application.Auth;
using Rentar.Modules.Identity.Application.Users;
using Rentar.Modules.Identity.Contracts;
using Rentar.Modules.Identity.Infrastructure.Persistence;
using Rentar.Modules.Identity.Infrastructure.Security;
using Rentar.Shared.Infrastructure.Modules;
using Rentar.Shared.Infrastructure.Persistence;
using Rentar.Shared.Presentation.Endpoints;

namespace Rentar.Modules.Identity.Infrastructure;

/// <summary>
/// Users, operation claims and authentication. Issues the JWTs every other module's endpoints are protected by.
/// </summary>
public sealed class IdentityModule : IModule
{
    public string Name => "Identity";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<IdentityDbContext>(IdentityDbContext.Schema);
        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        services.AddOptions<TokenOptions>().BindConfiguration(TokenOptions.SectionName);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<TokenOptions>>((options, tokenOptions) =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidIssuer = tokenOptions.Value.Issuer,
                    ValidAudience = tokenOptions.Value.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.Value.SecurityKey)
                };
            });
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<EfUserDal>().As<IUserDal>().SingleInstance();
        builder.RegisterType<JwtHelper>().As<ITokenHelper>().SingleInstance();
        builder.RegisterType<IdentityModuleApi>().As<IIdentityModuleApi>().SingleInstance();

        builder.RegisterUseCases(typeof(GetUsers).Assembly);
    }
}
