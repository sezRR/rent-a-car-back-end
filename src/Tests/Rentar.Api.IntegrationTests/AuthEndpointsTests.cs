using Rentar.Shared.Presentation.Endpoints;
using Rentar.Modules.Identity.Application.Auth;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests;

public class AuthEndpointsTests(RentarApiFactory factory) : IntegrationTestBase(factory)
{
    private const string Email = "driver@example.com";
    private const string Password = "Passw0rd!";

    [Fact]
    public async Task Register_ReturnsAccessToken()
    {
        var response = await Client.PostJsonAsync("/api/v1/auth/register", NewUser());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.ReadAsAsync<V1Response<AccessToken>>();
        Assert.True(result.Success);
        Assert.False(string.IsNullOrWhiteSpace(result.Data.Token));
        Assert.Equal(3, result.Data.Token.Split('.').Length);
    }

    [Fact]
    public async Task Register_WithAlreadyUsedEmail_ReturnsBadRequest()
    {
        await Client.PostJsonAsync("/api/v1/auth/register", NewUser());

        var response = await Client.PostJsonAsync("/api/v1/auth/register", NewUser());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.ReadAsAsync<V1Response>();
        Assert.False(result.Success);
        Assert.Equal("User already registered", result.Message);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsAccessToken()
    {
        await Client.PostJsonAsync("/api/v1/auth/register", NewUser());

        var response = await Client.PostJsonAsync("/api/v1/auth/login", new UserForLoginDto
        {
            Email = Email,
            Password = Password
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.ReadAsAsync<V1Response<AccessToken>>();
        Assert.False(string.IsNullOrWhiteSpace(result.Data.Token));
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsBadRequest()
    {
        await Client.PostJsonAsync("/api/v1/auth/register", NewUser());

        var response = await Client.PostJsonAsync("/api/v1/auth/login", new UserForLoginDto
        {
            Email = Email,
            Password = "wrong-password"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Password is wrong", await response.ReadTextAsync());
    }

    [Fact]
    public async Task Login_WithUnknownEmail_ReturnsBadRequest()
    {
        var response = await Client.PostJsonAsync("/api/v1/auth/login", new UserForLoginDto
        {
            Email = "nobody@example.com",
            Password = Password
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("User not found", await response.ReadTextAsync());
    }

    private static UserForRegisterDto NewUser()
    {
        return new UserForRegisterDto
        {
            FirstName = "Sezer",
            LastName = "Tetik",
            Email = Email,
            Password = Password,
            FindeksRating = 900
        };
    }
}
