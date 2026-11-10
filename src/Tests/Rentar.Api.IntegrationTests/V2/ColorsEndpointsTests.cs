using Rentar.Modules.Fleet.Application;
using Rentar.Modules.Fleet.Domain;
using Rentar.Modules.Fleet.Presentation.Endpoints.V2;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests.V2;

public class ColorsEndpointsTests(RentarApiFactory factory) : V2TestBase(factory)
{
    [Fact]
    public async Task Color_CanBeCreatedReadReplacedAndDeleted()
    {
        var color = await CreateColorAsync("Blu");
        Assert.True(color.Id > 0);

        var replaced = await Client.PutJsonAsync($"/api/v2/colors/{color.Id}", new ColorRequest("Blue"));
        Assert.Equal(HttpStatusCode.NoContent, replaced.StatusCode);

        var fetched = await Client.GetJsonAsync($"/api/v2/colors/{color.Id}");
        Assert.Equal("Blue", (await fetched.ReadAsAsync<Color>()).ColorName);

        var deleted = await Client.DeleteResourceAsync($"/api/v2/colors/{color.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);

        var colors = await (await Client.GetJsonAsync("/api/v2/colors")).ReadAsAsync<List<Color>>();
        Assert.Empty(colors);
    }

    [Fact]
    public async Task Update_WithNameShorterThanTwoCharacters_ReturnsProblemWithTheServiceMessage()
    {
        var color = await CreateColorAsync("Red");

        var response = await Client.PutJsonAsync($"/api/v2/colors/{color.Id}", new ColorRequest("R"));

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, Messages.ColorNameInvalid);
    }
}
