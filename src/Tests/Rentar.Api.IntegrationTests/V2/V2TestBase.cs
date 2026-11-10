using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Rentar.Modules.Fleet.Application.CarImages;
using Rentar.Modules.Fleet.Domain;
using Rentar.Modules.Fleet.Infrastructure.Storage;
using Rentar.Modules.Fleet.Presentation.Endpoints.V2;
using Rentar.Modules.Identity.Presentation.Endpoints.V2;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests.V2;

/// <summary>
/// Creates test data through the v2 API itself and checks its problem responses.
/// </summary>
public abstract class V2TestBase(RentarApiFactory factory) : IntegrationTestBase(factory)
{
    protected const string Password = "Passw0rd!";

    protected static readonly byte[] PngBytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00];
    protected static readonly byte[] JpegBytes = [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10];
    protected static readonly byte[] TextBytes = Encoding.UTF8.GetBytes("# not an image");

    /// <summary>
    /// Some lists are closed during the 22:00 maintenance hour, so tests that read them pin the clock to midday.
    /// </summary>
    protected void FreezeClockAtNoon()
    {
        Factory.Clock.FreezeAt(new DateTimeOffset(DateTime.Today.AddHours(12)));
    }

    protected void FreezeClockInMaintenanceHour()
    {
        Factory.Clock.FreezeAt(new DateTimeOffset(DateTime.Today.AddHours(22).AddMinutes(30)));
    }

    protected async Task<T> CreateAsync<T>(string url, object body)
    {
        var response = await Client.PostJsonAsync(url, body);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await response.ReadAsAsync<T>();
    }

    protected Task<Brand> CreateBrandAsync(string brandName = "Toyota")
    {
        return CreateAsync<Brand>("/api/v2/brands", new BrandRequest(brandName));
    }

    protected Task<Color> CreateColorAsync(string colorName = "Red")
    {
        return CreateAsync<Color>("/api/v2/colors", new ColorRequest(colorName));
    }

    protected async Task<CarResponse> CreateCarAsync(string brandName = "Toyota", string colorName = "Red", string description = "Corolla 1.6")
    {
        var brand = await CreateBrandAsync(brandName);
        var color = await CreateColorAsync(colorName);

        return await CreateCarAsync(brand.Id, color.Id, description);
    }

    protected Task<CarResponse> CreateCarAsync(int brandId, int colorId, string description = "Corolla 1.6")
    {
        return CreateAsync<CarResponse>("/api/v2/cars", new CarRequest(brandId, colorId, 2021, 950m, description, 700));
    }

    protected Task<UserResponse> CreateUserAsync(string email, string firstName = "Sezer", int findeksRating = 900)
    {
        return CreateAsync<UserResponse>("/api/v2/users", new UserRequest(firstName, "Tetik", email, Password, findeksRating));
    }

    protected async Task<HttpResponseMessage> UploadImageAsync(int carId, string fileName, byte[] bytes)
    {
        using var form = new MultipartFormDataContent
        {
            { new ByteArrayContent(bytes), "file", fileName }
        };

        return await Client.PostAsync($"/api/v2/cars/{carId}/images", form, TestContext.Current.CancellationToken);
    }

    protected string UploadedFile(string fileName)
    {
        var storage = (LocalImageStorage)Factory.Services.GetRequiredService<IImageStorage>();
        return Path.Combine(storage.UploadsPath, fileName);
    }

    /// <summary>
    /// Asserts an RFC 9457 problem response with the given status and, when one is given, detail.
    /// </summary>
    protected static async Task<ProblemDetails> AssertProblemAsync(HttpResponseMessage response, HttpStatusCode status, string detail = null)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var problem = await response.ReadAsAsync<ProblemDetails>();
        Assert.Equal((int)status, problem.Status);

        if (detail != null)
        {
            Assert.Equal(detail, problem.Detail);
        }

        return problem;
    }

    protected static async Task<HttpValidationProblemDetails> AssertValidationProblemAsync(HttpResponseMessage response, params string[] fields)
    {
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var problem = await response.ReadAsAsync<HttpValidationProblemDetails>();
        foreach (var field in fields)
        {
            Assert.Contains(field, problem.Errors.Keys);
        }

        return problem;
    }
}
