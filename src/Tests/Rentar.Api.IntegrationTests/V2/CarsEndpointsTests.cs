using Rentar.Modules.Fleet.Domain;
using Rentar.Modules.Fleet.Presentation.Endpoints.V2;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests.V2;

public class CarsEndpointsTests(RentarApiFactory factory) : V2TestBase(factory)
{
    [Fact]
    public async Task Create_ReturnsTheCarWithItsBrandAndColor()
    {
        var brand = await CreateBrandAsync("Toyota");
        var color = await CreateColorAsync("Red");

        var response = await Client.PostJsonAsync("/api/v2/cars", new CarRequest(brand.Id, color.Id, 2021, 950m, "Corolla 1.6", 700));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var car = await response.ReadAsAsync<CarResponse>();
        Assert.Equal($"/api/v2/cars/{car.Id}", response.Headers.Location?.OriginalString);
        Assert.Equal(new CarResponse(car.Id, brand.Id, "Toyota", color.Id, "Red", 2021, 950m, "Corolla 1.6", 700, null), car);

        var fetched = await Client.GetJsonAsync($"/api/v2/cars/{car.Id}");
        Assert.Equal(car, await fetched.ReadAsAsync<CarResponse>());
    }

    [Fact]
    public async Task GetAll_FiltersByBrandColorOrBoth()
    {
        var toyota = await CreateBrandAsync("Toyota");
        var fiat = await CreateBrandAsync("Fiat");
        var red = await CreateColorAsync("Red");
        var white = await CreateColorAsync("White");

        await CreateCarAsync(toyota.Id, red.Id, "Red Corolla");
        await CreateCarAsync(toyota.Id, white.Id, "White Corolla");
        await CreateCarAsync(fiat.Id, red.Id, "Red Egea");

        Assert.Equal(["Red Corolla", "Red Egea", "White Corolla"], await DescriptionsAsync("/api/v2/cars"));
        Assert.Equal(["Red Corolla", "White Corolla"], await DescriptionsAsync($"/api/v2/cars?brandId={toyota.Id}"));
        Assert.Equal(["Red Corolla", "Red Egea"], await DescriptionsAsync($"/api/v2/cars?colorId={red.Id}"));
        Assert.Equal(["Red Egea"], await DescriptionsAsync($"/api/v2/cars?brandId={fiat.Id}&colorId={red.Id}"));
    }

    [Fact]
    public async Task Update_ReplacesTheCar()
    {
        var car = await CreateCarAsync();
        var white = await CreateColorAsync("White");

        var response = await Client.PutJsonAsync($"/api/v2/cars/{car.Id}", new CarRequest(car.BrandId, white.Id, 2024, 1200m, "Corolla Hybrid", 800));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var fetched = await (await Client.GetJsonAsync($"/api/v2/cars/{car.Id}")).ReadAsAsync<CarResponse>();
        Assert.Equal(new CarResponse(car.Id, car.BrandId, "Toyota", white.Id, "White", 2024, 1200m, "Corolla Hybrid", 800, null), fetched);
    }

    [Fact]
    public async Task Delete_RemovesTheCarWithItsImagesAndTheirFiles()
    {
        var car = await CreateCarAsync();
        var image = await (await UploadImageAsync(car.Id, "car.png", PngBytes)).ReadAsAsync<CarImage>();
        var stored = UploadedFile(image.ImagePath);
        Assert.True(File.Exists(stored));

        var response = await Client.DeleteResourceAsync($"/api/v2/cars/{car.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        await AssertProblemAsync(await Client.GetJsonAsync($"/api/v2/cars/{car.Id}"), HttpStatusCode.NotFound);
        Assert.Empty(await (await Client.GetJsonAsync($"/api/v2/cars/{car.Id}/images")).ReadAsAsync<List<CarImage>>());
        Assert.False(File.Exists(stored));
    }

    [Theory]
    [InlineData("GET")]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    public async Task UnknownCar_IsNotFound(string method)
    {
        var response = method switch
        {
            "GET" => await Client.GetJsonAsync("/api/v2/cars/404"),
            "PUT" => await Client.PutJsonAsync("/api/v2/cars/404", new CarRequest(1, 1, 2021, 950m, "Corolla 1.6", 700)),
            _ => await Client.DeleteResourceAsync("/api/v2/cars/404")
        };

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "Car 404 was not found.");
    }

    [Fact]
    public async Task Create_WithoutPriceOrDescription_ReturnsValidationProblem()
    {
        var response = await Client.PostJsonAsync("/api/v2/cars", new CarRequest(1, 1, 2021, 0m, "", 700));

        await AssertValidationProblemAsync(response, "dailyPrice", "description");
    }

    private async Task<List<string>> DescriptionsAsync(string url)
    {
        var response = await Client.GetJsonAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var cars = await response.ReadAsAsync<List<CarResponse>>();
        return cars.Select(c => c.Description).Order().ToList();
    }
}
