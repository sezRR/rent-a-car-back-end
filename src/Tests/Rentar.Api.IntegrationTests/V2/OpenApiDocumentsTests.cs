using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests.V2;

public class OpenApiDocumentsTests(RentarApiFactory factory) : IntegrationTestBase(factory)
{
    [Theory]
    [InlineData("v1", "/api/v1/brands/getall")]
    [InlineData("v2", "/api/v2/brands/{id}")]
    public async Task EachVersion_HasItsOwnDocument_WithOnlyItsOwnPaths(string version, string expectedPath)
    {
        var response = await Client.GetJsonAsync($"/openapi/{version}.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(await response.ReadTextAsync());
        var paths = document.RootElement.GetProperty("paths").EnumerateObject().Select(p => p.Name).ToList();

        Assert.Contains(expectedPath, paths);
        Assert.All(paths, path => Assert.StartsWith($"/api/{version}/", path));
    }
}
