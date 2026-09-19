using Core.Extensions;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace WebAPI.IntegrationTests
{
    public class CarsEndpointsTests : IntegrationTestBase
    {
        public CarsEndpointsTests(RentarApiFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Add_PersistsCar_AndGetAllReturnsIt()
        {
            var response = await Client.PostJsonAsync("/api/cars/add", NewCar());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var cars = await Client.GetJsonAsync("/api/cars/getall");
            var listed = await cars.ReadAsAsync<DataResult<List<Car>>>();

            var car = Assert.Single(listed.Data);
            Assert.Equal("Corolla 1.6", car.Description);
            Assert.Equal(950m, car.DailyPrice);
        }

        [Fact]
        public async Task Add_WithEmptyDescriptionAndZeroPrice_ReturnsValidationErrors()
        {
            var invalidCar = new Car
            {
                BrandId = 1,
                ColorId = 1,
                ModelYear = 2020,
                DailyPrice = 0,
                Description = string.Empty,
                MinimumFindeksRating = 500
            };

            var response = await Client.PostJsonAsync("/api/cars/add", invalidCar);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var error = await response.ReadAsAsync<ValidationErrorDetails>();
            Assert.Equal(400, error.StatusCode);
            Assert.Contains(error.Errors, e => e.PropertyName == "Description");
            Assert.Contains(error.Errors, e => e.PropertyName == "DailyPrice");
        }

        [Fact]
        public async Task GetCarDetails_JoinsBrandAndColor()
        {
            var brandId = await AddBrandAsync("Toyota");
            var colorId = await AddColorAsync("Red");
            await Client.PostJsonAsync("/api/cars/add", NewCar(brandId, colorId));

            var response = await Client.GetJsonAsync("/api/cars/getcardetails");
            var details = await response.ReadAsAsync<DataResult<List<CarDetailDto>>>();

            var detail = Assert.Single(details.Data);
            Assert.Equal("Toyota", detail.BrandName);
            Assert.Equal("Red", detail.ColorName);
            Assert.Equal("Corolla 1.6", detail.Description);
        }

        [Fact]
        public async Task GetCarsByBrandId_IsRefreshedAfterAddingAnotherCar()
        {
            var brandId = await AddBrandAsync("Toyota");
            var colorId = await AddColorAsync("Red");
            await Client.PostJsonAsync("/api/cars/add", NewCar(brandId, colorId));

            // First call fills the cache that the caching aspect keeps for this method.
            var firstCall = await Client.GetJsonAsync($"/api/cars/getcarsbybrandid?brandId={brandId}");
            var firstResult = await firstCall.ReadAsAsync<DataResult<List<CarDetailDto>>>();
            Assert.Single(firstResult.Data);

            await Client.PostJsonAsync("/api/cars/add", NewCar(brandId, colorId, "Corolla 1.8"));

            var secondCall = await Client.GetJsonAsync($"/api/cars/getcarsbybrandid?brandId={brandId}");
            var secondResult = await secondCall.ReadAsAsync<DataResult<List<CarDetailDto>>>();

            Assert.Equal(2, secondResult.Data.Count);
            Assert.Contains(secondResult.Data, c => c.Description == "Corolla 1.8");
        }

        [Fact]
        public async Task GetCarsByColorId_ReturnsOnlyMatchingCars()
        {
            var brandId = await AddBrandAsync("Toyota");
            var red = await AddColorAsync("Red");
            var blue = await AddColorAsync("Blue");
            await Client.PostJsonAsync("/api/cars/add", NewCar(brandId, red, "Red car"));
            await Client.PostJsonAsync("/api/cars/add", NewCar(brandId, blue, "Blue car"));

            var response = await Client.GetJsonAsync($"/api/cars/getcarsbycolorid?colorId={blue}");
            var result = await response.ReadAsAsync<DataResult<List<CarDetailDto>>>();

            var car = Assert.Single(result.Data);
            Assert.Equal("Blue car", car.Description);
        }

        private static Car NewCar(int brandId = 1, int colorId = 1, string description = "Corolla 1.6")
        {
            return new Car
            {
                BrandId = brandId,
                ColorId = colorId,
                ModelYear = 2021,
                DailyPrice = 950m,
                Description = description,
                MinimumFindeksRating = 700
            };
        }

        private async Task<int> AddBrandAsync(string brandName)
        {
            await Client.PostJsonAsync("/api/brands/add", new Brand { BrandName = brandName });

            var brands = await Client.GetJsonAsync("/api/brands/getall");
            var listed = await brands.ReadAsAsync<DataResult<List<Brand>>>();

            return listed.Data.Single(b => b.BrandName == brandName).Id;
        }

        private async Task<int> AddColorAsync(string colorName)
        {
            await Client.PostJsonAsync("/api/colors/add", new Color { ColorName = colorName });

            var colors = await Client.GetJsonAsync("/api/colors/getall");
            var listed = await colors.ReadAsAsync<DataResult<List<Color>>>();

            return listed.Data.Single(c => c.ColorName == colorName).Id;
        }
    }
}
