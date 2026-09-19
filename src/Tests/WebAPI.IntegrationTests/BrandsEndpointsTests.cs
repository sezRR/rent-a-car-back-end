using Core.Extensions;
using Core.Utilities.Results;
using Entities.Concrete;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace WebAPI.IntegrationTests
{
    public class BrandsEndpointsTests : IntegrationTestBase
    {
        public BrandsEndpointsTests(RentarApiFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Add_PersistsBrand_AndGetAllReturnsIt()
        {
            var response = await Client.PostJsonAsync("/api/brands/add", new Brand { BrandName = "Toyota" });
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var added = await response.ReadAsAsync<Result>();
            Assert.True(added.Success);
            Assert.Equal("Brand Added", added.Message);

            var brands = await Client.GetJsonAsync("/api/brands/getall");
            var listed = await brands.ReadAsAsync<DataResult<List<Brand>>>();

            Assert.True(listed.Success);
            var brand = Assert.Single(listed.Data);
            Assert.Equal("Toyota", brand.BrandName);
            Assert.True(brand.Id > 0);
        }

        [Fact]
        public async Task GetById_ReturnsRequestedBrand()
        {
            await Client.PostJsonAsync("/api/brands/add", new Brand { BrandName = "Honda" });
            await Client.PostJsonAsync("/api/brands/add", new Brand { BrandName = "Renault" });
            var id = await GetBrandIdAsync("Renault");

            var response = await Client.GetJsonAsync($"/api/brands/getbyid?id={id}");
            var result = await response.ReadAsAsync<DataResult<Brand>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Renault", result.Data.BrandName);
        }

        [Fact]
        public async Task Update_ChangesBrandName()
        {
            await Client.PostJsonAsync("/api/brands/add", new Brand { BrandName = "Ople" });
            var id = await GetBrandIdAsync("Ople");

            var response = await Client.PostJsonAsync("/api/brands/update", new Brand { Id = id, BrandName = "Opel" });
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updated = await Client.GetJsonAsync($"/api/brands/getbyid?id={id}");
            var result = await updated.ReadAsAsync<DataResult<Brand>>();

            Assert.Equal("Opel", result.Data.BrandName);
        }

        [Fact]
        public async Task Delete_RemovesBrand()
        {
            await Client.PostJsonAsync("/api/brands/add", new Brand { BrandName = "Fiat" });
            var id = await GetBrandIdAsync("Fiat");

            var response = await Client.PostJsonAsync("/api/brands/delete", new Brand { Id = id, BrandName = "Fiat" });
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var brands = await Client.GetJsonAsync("/api/brands/getall");
            var listed = await brands.ReadAsAsync<DataResult<List<Brand>>>();

            Assert.Empty(listed.Data);
        }

        [Fact]
        public async Task Add_WithNameShorterThanTwoCharacters_ReturnsValidationError()
        {
            var response = await Client.PostJsonAsync("/api/brands/add", new Brand { BrandName = "A" });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var error = await response.ReadAsAsync<ValidationErrorDetails>();
            Assert.Equal(400, error.StatusCode);
            Assert.Contains(error.Errors, e => e.PropertyName == "BrandName");

            var brands = await Client.GetJsonAsync("/api/brands/getall");
            var listed = await brands.ReadAsAsync<DataResult<List<Brand>>>();
            Assert.Empty(listed.Data);
        }

        private async Task<int> GetBrandIdAsync(string brandName)
        {
            var brands = await Client.GetJsonAsync("/api/brands/getall");
            var listed = await brands.ReadAsAsync<DataResult<List<Brand>>>();

            return listed.Data.Find(b => b.BrandName == brandName).Id;
        }
    }
}
