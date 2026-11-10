using Rentar.Modules.Fleet.Application;
using Rentar.Modules.Fleet.Application.CarImages;
using Rentar.Modules.Fleet.Domain;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests.V2;

public class CarImagesEndpointsTests(RentarApiFactory factory) : V2TestBase(factory)
{
    [Fact]
    public async Task Upload_AnswersCreated_WithTheImageUnderItsCar()
    {
        var car = await CreateCarAsync();

        var response = await UploadImageAsync(car.Id, "car.png", PngBytes);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var image = await response.ReadAsAsync<CarImage>();
        Assert.Equal(car.Id, image.CarId);
        Assert.NotEqual(IImageStorage.DefaultImageName, image.ImagePath);
        Assert.Equal($"/api/v2/cars/{car.Id}/images/{image.Id}", response.Headers.Location?.OriginalString);
        Assert.True(File.Exists(UploadedFile(image.ImagePath)));

        var fetched = await Client.GetJsonAsync(response.Headers.Location!.OriginalString);
        Assert.Equal(image.ImagePath, (await fetched.ReadAsAsync<CarImage>()).ImagePath);

        await Client.DeleteResourceAsync($"/api/v2/cars/{car.Id}/images");
    }

    [Fact]
    public async Task Upload_WithoutFile_GivesTheCarTheDefaultImage()
    {
        var car = await CreateCarAsync();
        using var form = new MultipartFormDataContent { { new StringContent("no file"), "note" } };

        var response = await Client.PostAsync($"/api/v2/cars/{car.Id}/images", form, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(IImageStorage.DefaultImageName, (await response.ReadAsAsync<CarImage>()).ImagePath);
    }

    [Theory]
    [InlineData("notes.md")]
    [InlineData("fake.png")]
    public async Task Upload_OfAnythingButPngOrJpeg_IsRejected(string fileName)
    {
        var car = await CreateCarAsync();

        var response = await UploadImageAsync(car.Id, fileName, TextBytes);

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, Messages.CarImageInvalidFile);
        Assert.Empty(await ImagesAsync(car.Id));
    }

    [Fact]
    public async Task Replace_SwapsTheStoredFile()
    {
        var car = await CreateCarAsync();
        var original = await (await UploadImageAsync(car.Id, "car.png", PngBytes)).ReadAsAsync<CarImage>();

        using var form = new MultipartFormDataContent { { new ByteArrayContent(JpegBytes), "file", "new.jpg" } };
        var response = await Client.PutAsync($"/api/v2/cars/{car.Id}/images/{original.Id}", form, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var replaced = Assert.Single(await ImagesAsync(car.Id));
        Assert.Equal(original.Id, replaced.Id);
        Assert.False(File.Exists(UploadedFile(original.ImagePath)));
        Assert.True(File.Exists(UploadedFile(replaced.ImagePath)));

        await Client.DeleteResourceAsync($"/api/v2/cars/{car.Id}/images");
    }

    [Fact]
    public async Task Replace_WithoutFile_IsRejected()
    {
        var car = await CreateCarAsync();
        var image = await (await UploadImageAsync(car.Id, "car.png", PngBytes)).ReadAsAsync<CarImage>();

        using var form = new MultipartFormDataContent { { new StringContent("no file"), "note" } };
        var response = await Client.PutAsync($"/api/v2/cars/{car.Id}/images/{image.Id}", form, TestContext.Current.CancellationToken);

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, Messages.CarImageInvalidFile);
        Assert.True(File.Exists(UploadedFile(image.ImagePath)));

        await Client.DeleteResourceAsync($"/api/v2/cars/{car.Id}/images");
    }

    [Fact]
    public async Task Delete_RemovesOnlyThatImageAndItsFile()
    {
        var car = await CreateCarAsync();
        var removed = await (await UploadImageAsync(car.Id, "first.png", PngBytes)).ReadAsAsync<CarImage>();
        var kept = await (await UploadImageAsync(car.Id, "second.jpg", JpegBytes)).ReadAsAsync<CarImage>();

        var response = await Client.DeleteResourceAsync($"/api/v2/cars/{car.Id}/images/{removed.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(kept.Id, Assert.Single(await ImagesAsync(car.Id)).Id);
        Assert.False(File.Exists(UploadedFile(removed.ImagePath)));
        Assert.True(File.Exists(UploadedFile(kept.ImagePath)));

        await Client.DeleteResourceAsync($"/api/v2/cars/{car.Id}/images");
    }

    [Fact]
    public async Task Image_IsNotFoundUnderAnotherCar()
    {
        var car = await CreateCarAsync("Toyota", "Red");
        var otherCar = await CreateCarAsync("Fiat", "White");
        var image = await (await UploadImageAsync(car.Id, "car.png", PngBytes)).ReadAsAsync<CarImage>();

        var response = await Client.GetJsonAsync($"/api/v2/cars/{otherCar.Id}/images/{image.Id}");
        var delete = await Client.DeleteResourceAsync($"/api/v2/cars/{otherCar.Id}/images/{image.Id}");

        await AssertProblemAsync(response, HttpStatusCode.NotFound, $"Car image {image.Id} was not found.");
        await AssertProblemAsync(delete, HttpStatusCode.NotFound);
        Assert.Single(await ImagesAsync(car.Id));

        await Client.DeleteResourceAsync($"/api/v2/cars/{car.Id}/images");
    }

    [Fact]
    public async Task DeleteAll_RemovesEveryImageOfTheCar_AndAlsoSucceedsWhenThereAreNone()
    {
        var car = await CreateCarAsync();
        var stored = UploadedFile((await (await UploadImageAsync(car.Id, "car.png", PngBytes)).ReadAsAsync<CarImage>()).ImagePath);
        await UploadImageAsync(car.Id, "car.jpg", JpegBytes);

        var first = await Client.DeleteResourceAsync($"/api/v2/cars/{car.Id}/images");
        var second = await Client.DeleteResourceAsync($"/api/v2/cars/{car.Id}/images");

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, second.StatusCode);
        Assert.Empty(await ImagesAsync(car.Id));
        Assert.False(File.Exists(stored));
    }

    [Fact]
    public async Task CarImages_ListsTheImagesOfEveryCar()
    {
        var car = await CreateCarAsync("Toyota", "Red");
        var otherCar = await CreateCarAsync("Fiat", "White");
        await UploadImageAsync(car.Id, "car.png", PngBytes);
        await UploadImageAsync(otherCar.Id, "car.jpg", JpegBytes);

        var response = await Client.GetJsonAsync("/api/v2/car-images");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var images = await response.ReadAsAsync<List<CarImage>>();
        Assert.Equal([car.Id, otherCar.Id], images.Select(i => i.CarId).Order());

        await Client.DeleteResourceAsync($"/api/v2/cars/{car.Id}/images");
        await Client.DeleteResourceAsync($"/api/v2/cars/{otherCar.Id}/images");
    }

    private async Task<List<CarImage>> ImagesAsync(int carId)
    {
        var response = await Client.GetJsonAsync($"/api/v2/cars/{carId}/images");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.ReadAsAsync<List<CarImage>>();
    }
}
