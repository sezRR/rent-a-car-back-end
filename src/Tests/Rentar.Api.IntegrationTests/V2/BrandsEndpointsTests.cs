using Rentar.Modules.Fleet.Domain;
using Rentar.Modules.Fleet.Presentation.Endpoints.V2;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests.V2;

public class BrandsEndpointsTests(RentarApiFactory factory) : V2TestBase(factory)
{
    [Fact]
    public async Task Create_AnswersCreated_WithTheBrandAndWhereItLives()
    {
        var response = await Client.PostJsonAsync("/api/v2/brands", new BrandRequest("Toyota"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.ReadAsAsync<Brand>();
        Assert.Equal("Toyota", created.BrandName);
        Assert.Equal($"/api/v2/brands/{created.Id}", response.Headers.Location?.OriginalString);

        var fetched = await Client.GetJsonAsync(response.Headers.Location!.OriginalString);
        Assert.Equal(HttpStatusCode.OK, fetched.StatusCode);
        Assert.Equal("Toyota", (await fetched.ReadAsAsync<Brand>()).BrandName);
    }

    [Fact]
    public async Task GetAll_ReturnsThePlainList_WithoutTheV1Envelope()
    {
        await CreateBrandAsync("Honda");
        await CreateBrandAsync("Renault");

        var response = await Client.GetJsonAsync("/api/v2/brands");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.StartsWith("[", await response.ReadTextAsync());
        var brands = await response.ReadAsAsync<List<Brand>>();
        Assert.Equal(["Honda", "Renault"], brands.ConvertAll(b => b.BrandName));
    }

    [Fact]
    public async Task Update_ReplacesTheBrand()
    {
        var brand = await CreateBrandAsync("Ople");

        var response = await Client.PutJsonAsync($"/api/v2/brands/{brand.Id}", new BrandRequest("Opel"));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var fetched = await Client.GetJsonAsync($"/api/v2/brands/{brand.Id}");
        Assert.Equal("Opel", (await fetched.ReadAsAsync<Brand>()).BrandName);
    }

    [Fact]
    public async Task Delete_RemovesTheBrand()
    {
        var brand = await CreateBrandAsync("Fiat");

        var response = await Client.DeleteResourceAsync($"/api/v2/brands/{brand.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        await AssertProblemAsync(await Client.GetJsonAsync($"/api/v2/brands/{brand.Id}"), HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("GET")]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    public async Task UnknownBrand_IsNotFound(string method)
    {
        var response = method switch
        {
            "GET" => await Client.GetJsonAsync("/api/v2/brands/404"),
            "PUT" => await Client.PutJsonAsync("/api/v2/brands/404", new BrandRequest("Opel")),
            _ => await Client.DeleteResourceAsync("/api/v2/brands/404")
        };

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "Brand 404 was not found.");
    }

    [Fact]
    public async Task Create_WithNameShorterThanTwoCharacters_ReturnsValidationProblem()
    {
        var response = await Client.PostJsonAsync("/api/v2/brands", new BrandRequest("A"));

        await AssertValidationProblemAsync(response, "brandName");
        var brands = await (await Client.GetJsonAsync("/api/v2/brands")).ReadAsAsync<List<Brand>>();
        Assert.Empty(brands);
    }
}
