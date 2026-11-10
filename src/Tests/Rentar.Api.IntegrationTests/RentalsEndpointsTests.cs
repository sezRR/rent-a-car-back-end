using Rentar.Modules.Customers.Domain;
using Rentar.Modules.Fleet.Domain;
using Rentar.Modules.Identity.Application.Auth;
using Rentar.Modules.Identity.Domain;
using Rentar.Modules.Rentals.Application;
using Rentar.Modules.Rentals.Domain;
using Rentar.Shared.Presentation.Endpoints;
using Rentar.Shared.Presentation.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests;

public class RentalsEndpointsTests(RentarApiFactory factory) : IntegrationTestBase(factory)
{
    private static readonly DateTime RentDate = new DateTime(2026, 9, 18, 10, 0, 0, DateTimeKind.Unspecified);
    private static readonly DateTime ReturnDate = new DateTime(2026, 9, 20, 10, 0, 0, DateTimeKind.Unspecified);

    [Fact]
    public async Task Add_StoresLocalDateTimes_AndGetAllReturnsThem()
    {
        var response = await Client.PostJsonAsync("/api/v1/rentals/add", NewRental());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rentals = await Client.GetJsonAsync("/api/v1/rentals/getall");
        var listed = await rentals.ReadAsAsync<V1Response<List<Rental>>>();

        var rental = Assert.Single(listed.Data);
        Assert.Equal(RentDate, rental.RentDate);
        Assert.Equal(ReturnDate, rental.ReturnDate);
        Assert.Equal(1, rental.CarId);
    }

    [Fact]
    public async Task GetRentalByCarId_ReturnsRentalOfThatCar()
    {
        await Client.PostJsonAsync("/api/v1/rentals/add", NewRental(carId: 1));
        await Client.PostJsonAsync("/api/v1/rentals/add", NewRental(carId: 2, customerId: 7));

        var response = await Client.GetJsonAsync("/api/v1/rentals/getrentalbycarid?carId=2");
        var result = await response.ReadAsAsync<V1Response<Rental>>();

        Assert.Equal(2, result.Data.CarId);
        Assert.Equal(7, result.Data.CustomerId);
    }

    [Fact]
    public async Task Add_WithoutDates_ReturnsValidationErrors()
    {
        var response = await Client.PostJsonAsync("/api/v1/rentals/add", new Rental
        {
            CarId = 1,
            CustomerId = 1
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.ReadAsAsync<ValidationErrorDetails>();
        Assert.Contains(error.Errors, e => e.PropertyName == "RentDate");
        Assert.Contains(error.Errors, e => e.PropertyName == "ReturnDate");
    }

    [Fact]
    public async Task GetRentalDetails_CombinesCustomerUserAndCarFromTheirModules()
    {
        // Rental details are refused during the 22:00 maintenance hour.
        Factory.Clock.FreezeAt(new DateTimeOffset(DateTime.Today.AddHours(12)));

        var userId = await RegisterUserAsync("Sezer", "renter@example.com");
        var customerId = await AddCustomerAsync(userId, "Rentar Logistics");
        var carId = await AddCarAsync("Toyota", "Red", "Corolla 1.6");

        await Client.PostJsonAsync("/api/v1/rentals/add", NewRental(carId, customerId));
        // Points at a car Fleet does not know, so it has no details to show.
        await Client.PostJsonAsync("/api/v1/rentals/add", NewRental(carId: 999, customerId: customerId));

        var response = await Client.GetJsonAsync("/api/v1/rentals/getrentaldetails");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var details = await response.ReadAsAsync<V1Response<List<RentalDetailDto>>>();
        var detail = Assert.Single(details.Data);
        Assert.Equal(carId, detail.CarId);
        Assert.Equal("Toyota", detail.BrandName);
        Assert.Equal("Red", detail.ColorName);
        Assert.Equal("Corolla 1.6", detail.Description);
        Assert.Equal(950m, detail.DailyPrice);
        Assert.Equal("Rentar Logistics", detail.CompanyName);
        Assert.Equal("Sezer", detail.UserName);
        Assert.Equal(RentDate, detail.RentDate);
    }

    private async Task<int> RegisterUserAsync(string firstName, string email)
    {
        await Client.PostJsonAsync("/api/v1/auth/register", new RegisterUser(firstName, "Tetik", email, "Passw0rd!", 900));

        var response = await Client.GetJsonAsync($"/api/v1/users/getbymail?email={email}");
        return (await response.ReadAsAsync<V1Response<User>>()).Data.Id;
    }

    private async Task<int> AddCustomerAsync(int userId, string companyName)
    {
        await Client.PostJsonAsync("/api/v1/customers/add", new Customer { UserId = userId, CompanyName = companyName });

        var response = await Client.GetJsonAsync($"/api/v1/customers/getcustomersbyuserid?userId={userId}");
        return (await response.ReadAsAsync<V1Response<List<Customer>>>()).Data.Single().Id;
    }

    private async Task<int> AddCarAsync(string brandName, string colorName, string description)
    {
        await Client.PostJsonAsync("/api/v1/brands/add", new Brand { BrandName = brandName });
        await Client.PostJsonAsync("/api/v1/colors/add", new Color { ColorName = colorName });

        var brands = await (await Client.GetJsonAsync("/api/v1/brands/getall")).ReadAsAsync<V1Response<List<Brand>>>();
        var colors = await (await Client.GetJsonAsync("/api/v1/colors/getall")).ReadAsAsync<V1Response<List<Color>>>();

        await Client.PostJsonAsync("/api/v1/cars/add", new Car
        {
            BrandId = brands.Data.Single().Id,
            ColorId = colors.Data.Single().Id,
            ModelYear = 2021,
            DailyPrice = 950m,
            Description = description,
            MinimumFindeksRating = 700
        });

        var cars = await (await Client.GetJsonAsync("/api/v1/cars/getall")).ReadAsAsync<V1Response<List<Car>>>();
        return cars.Data.Single().Id;
    }

    private static Rental NewRental(int carId = 1, int customerId = 1)
    {
        return new Rental
        {
            CarId = carId,
            CustomerId = customerId,
            RentDate = RentDate,
            ReturnDate = ReturnDate
        };
    }
}
