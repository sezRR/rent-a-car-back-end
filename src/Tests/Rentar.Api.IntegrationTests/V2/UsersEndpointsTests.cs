using Rentar.Modules.Identity.Application;
using Rentar.Modules.Identity.Application.Auth;
using Rentar.Modules.Identity.Presentation.Endpoints.V2;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests.V2;

public class UsersEndpointsTests(RentarApiFactory factory) : V2TestBase(factory)
{
    private const string Email = "driver@example.com";

    [Fact]
    public async Task Create_AnswersCreated_WithoutThePasswordHashOrSalt()
    {
        var response = await Client.PostJsonAsync("/api/v2/users", new UserRequest("Sezer", "Tetik", Email, Password, 900));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var json = await response.ReadTextAsync();
        Assert.DoesNotContain("password", json, System.StringComparison.OrdinalIgnoreCase);

        var user = await response.ReadAsAsync<UserResponse>();
        Assert.Equal(new UserResponse(user.Id, "Sezer", "Tetik", Email, true, 900), user);
        Assert.Equal($"/api/v2/users/{user.Id}", response.Headers.Location?.OriginalString);

        var fetched = await Client.GetJsonAsync($"/api/v2/users/{user.Id}");
        Assert.DoesNotContain("password", await fetched.ReadTextAsync(), System.StringComparison.OrdinalIgnoreCase);
        Assert.Equal(user, await fetched.ReadAsAsync<UserResponse>());
    }

    [Fact]
    public async Task Create_WithAnEmailAlreadyInUse_IsAConflict()
    {
        await CreateUserAsync(Email);

        var response = await Client.PostJsonAsync("/api/v2/users", new UserRequest("Other", "Driver", Email, Password, 500));

        await AssertProblemAsync(response, HttpStatusCode.Conflict, Messages.UserAlreadyExists);
    }

    [Fact]
    public async Task Create_WithoutAName_ReturnsValidationProblem()
    {
        var response = await Client.PostJsonAsync("/api/v2/users", new UserRequest("", "", Email, Password, 900));

        await AssertValidationProblemAsync(response, "firstName", "lastName");
    }

    [Fact]
    public async Task GetAll_FiltersByEmailAndMinimumFindeksRating()
    {
        FreezeClockAtNoon();
        await CreateUserAsync("low@example.com", "Low", findeksRating: 400);
        await CreateUserAsync("high@example.com", "High", findeksRating: 1200);

        Assert.Equal(["High", "Low"], await FirstNamesAsync("/api/v2/users"));
        Assert.Equal(["Low"], await FirstNamesAsync("/api/v2/users?email=low@example.com"));
        Assert.Equal(["High"], await FirstNamesAsync("/api/v2/users?minFindeksRating=1000"));
        Assert.Empty(await FirstNamesAsync("/api/v2/users?email=low@example.com&minFindeksRating=1000"));
        Assert.Empty(await FirstNamesAsync("/api/v2/users?email=nobody@example.com"));
    }

    [Fact]
    public async Task GetAll_DuringTheMaintenanceHour_IsServiceUnavailable()
    {
        FreezeClockInMaintenanceHour();

        var response = await Client.GetJsonAsync("/api/v2/users");

        await AssertProblemAsync(response, HttpStatusCode.ServiceUnavailable, Messages.MaintenanceTime);
    }

    [Fact]
    public async Task Update_ReplacesTheUser_IncludingThePassword()
    {
        var user = await CreateUserAsync(Email);

        var response = await Client.PutJsonAsync($"/api/v2/users/{user.Id}", new UserRequest("Sezer", "Tetik", Email, "N3wPassw0rd!", 1000));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var fetched = await (await Client.GetJsonAsync($"/api/v2/users/{user.Id}")).ReadAsAsync<UserResponse>();
        Assert.Equal(1000, fetched.FindeksRating);

        var withNewPassword = await Client.PostJsonAsync("/api/v2/auth/tokens", new TokenRequest(Email, "N3wPassw0rd!"));
        var withOldPassword = await Client.PostJsonAsync("/api/v2/auth/tokens", new TokenRequest(Email, Password));
        Assert.Equal(HttpStatusCode.OK, withNewPassword.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, withOldPassword.StatusCode);
    }

    [Fact]
    public async Task UnknownUser_IsNotFound()
    {
        var get = await Client.GetJsonAsync("/api/v2/users/404");
        var put = await Client.PutJsonAsync("/api/v2/users/404", new UserRequest("Sezer", "Tetik", Email, Password, 900));

        await AssertProblemAsync(get, HttpStatusCode.NotFound, "User 404 was not found.");
        await AssertProblemAsync(put, HttpStatusCode.NotFound, "User 404 was not found.");
    }

    [Fact]
    public async Task CreateToken_WithValidCredentials_ReturnsAnAccessToken()
    {
        await CreateUserAsync(Email);

        var response = await Client.PostJsonAsync("/api/v2/auth/tokens", new TokenRequest(Email, Password));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var token = await response.ReadAsAsync<AccessToken>();
        Assert.Equal(3, token.Token.Split('.').Length);
    }

    [Fact]
    public async Task CreateToken_GivesTheSameAnswerForAWrongPasswordAndAnUnknownEmail()
    {
        await CreateUserAsync(Email);

        var wrongPassword = await Client.PostJsonAsync("/api/v2/auth/tokens", new TokenRequest(Email, "wrong-password"));
        var unknownEmail = await Client.PostJsonAsync("/api/v2/auth/tokens", new TokenRequest("nobody@example.com", Password));

        const string detail = "Email or password is wrong.";
        await AssertProblemAsync(wrongPassword, HttpStatusCode.Unauthorized, detail);
        await AssertProblemAsync(unknownEmail, HttpStatusCode.Unauthorized, detail);
    }

    private async Task<List<string>> FirstNamesAsync(string url)
    {
        var response = await Client.GetJsonAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var users = await response.ReadAsAsync<List<UserResponse>>();
        return users.Select(u => u.FirstName).Order().ToList();
    }
}
