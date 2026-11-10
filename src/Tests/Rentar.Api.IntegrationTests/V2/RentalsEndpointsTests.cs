using Rentar.Modules.Customers.Domain;
using Rentar.Modules.Customers.Presentation.Endpoints.V2;
using Rentar.Modules.Rentals.Application;
using Rentar.Modules.Rentals.Domain;
using Rentar.Modules.Rentals.Presentation.Endpoints.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests.V2;

public class RentalsEndpointsTests(RentarApiFactory factory) : V2TestBase(factory)
{
    private static readonly DateTime RentDate = new(2026, 9, 18, 10, 0, 0, DateTimeKind.Unspecified);
    private static readonly DateTime ReturnDate = new(2026, 9, 20, 10, 0, 0, DateTimeKind.Unspecified);

    [Fact]
    public async Task Create_AnswersCreated_WithTheRentalAndWhereItLives()
    {
        var response = await Client.PostJsonAsync("/api/v2/rentals", new RentalRequest(1, 3, RentDate, ReturnDate));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var rental = await response.ReadAsAsync<Rental>();
        Assert.Equal($"/api/v2/rentals/{rental.Id}", response.Headers.Location?.OriginalString);

        var fetched = await (await Client.GetJsonAsync($"/api/v2/rentals/{rental.Id}")).ReadAsAsync<Rental>();
        Assert.Equal(1, fetched.CarId);
        Assert.Equal(3, fetched.CustomerId);
        Assert.Equal(RentDate, fetched.RentDate);
        Assert.Equal(ReturnDate, fetched.ReturnDate);
    }

    [Fact]
    public async Task GetAll_FiltersByCar_AndReturnsEveryRentalOfIt()
    {
        // v1's getrentalbycarid expects at most one rental per car; the v2 filter lists them all.
        await CreateRentalAsync(carId: 1, customerId: 1);
        await CreateRentalAsync(carId: 2, customerId: 1, RentDate.AddDays(10), ReturnDate.AddDays(10));
        await CreateRentalAsync(carId: 2, customerId: 2, RentDate.AddDays(20), ReturnDate.AddDays(20));

        var all = await (await Client.GetJsonAsync("/api/v2/rentals")).ReadAsAsync<List<Rental>>();
        var ofCar = await (await Client.GetJsonAsync("/api/v2/rentals?carId=2")).ReadAsAsync<List<Rental>>();

        Assert.Equal(3, all.Count);
        Assert.Equal([1, 2], ofCar.Select(r => r.CustomerId).Order());
        Assert.All(ofCar, r => Assert.Equal(2, r.CarId));
    }

    [Theory]
    [InlineData("2026-09-19T10:00:00", "2026-09-22T10:00:00", false)]
    [InlineData("2026-09-10T10:00:00", "2026-09-18T10:00:00", false)]
    [InlineData("2026-09-21T10:00:00", "2026-09-25T10:00:00", true)]
    public async Task Availability_IsFalseOnlyWhenTheCarIsRentedDuringThePeriod(string from, string to, bool expected)
    {
        await CreateRentalAsync(carId: 1, customerId: 1);

        var response = await Client.GetJsonAsync($"/api/v2/rentals/availability?carId=1&rentDate={from}&returnDate={to}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var availability = await response.ReadAsAsync<RentalAvailabilityResponse>();
        Assert.Equal(expected, availability.Available);
        Assert.Equal(1, availability.CarId);
        Assert.Equal(DateTime.Parse(from), availability.RentDate);
    }

    [Fact]
    public async Task Availability_OfAnotherCar_IsNotAffected()
    {
        await CreateRentalAsync(carId: 1, customerId: 1);

        var response = await Client.GetJsonAsync($"/api/v2/rentals/availability?carId=2&rentDate=2026-09-19T10:00:00&returnDate=2026-09-19T12:00:00");

        Assert.True((await response.ReadAsAsync<RentalAvailabilityResponse>()).Available);
    }

    [Fact]
    public async Task Details_CombineCustomerUserAndCarFromTheirModules()
    {
        FreezeClockAtNoon();
        var user = await CreateUserAsync("renter@example.com", "Sezer");
        var customer = await CreateAsync<Customer>("/api/v2/customers", new CustomerRequest(user.Id, "Rentar Logistics"));
        var car = await CreateCarAsync("Toyota", "Red", "Corolla 1.6");
        await CreateRentalAsync(car.Id, customer.Id);
        // Points at a car Fleet does not know, so it has no details to show.
        await CreateRentalAsync(carId: 999, customer.Id);

        var response = await Client.GetJsonAsync("/api/v2/rentals/details");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var detail = Assert.Single(await response.ReadAsAsync<List<RentalDetailDto>>());
        Assert.Equal(car.Id, detail.CarId);
        Assert.Equal("Toyota", detail.BrandName);
        Assert.Equal("Red", detail.ColorName);
        Assert.Equal("Rentar Logistics", detail.CompanyName);
        Assert.Equal("Sezer", detail.UserName);
    }

    [Fact]
    public async Task Details_DuringTheMaintenanceHour_AreServiceUnavailable()
    {
        FreezeClockInMaintenanceHour();

        var response = await Client.GetJsonAsync("/api/v2/rentals/details");

        await AssertProblemAsync(response, HttpStatusCode.ServiceUnavailable, Messages.MaintenanceTime);
    }

    [Fact]
    public async Task Create_WithoutDates_ReturnsValidationProblem()
    {
        var response = await Client.PostJsonAsync("/api/v2/rentals", new { carId = 1, customerId = 1 });

        await AssertValidationProblemAsync(response, "rentDate", "returnDate");
    }

    [Fact]
    public async Task UnknownRental_IsNotFound()
    {
        var response = await Client.GetJsonAsync("/api/v2/rentals/404");

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "Rental 404 was not found.");
    }

    private Task<Rental> CreateRentalAsync(int carId, int customerId, DateTime? rentDate = null, DateTime? returnDate = null)
    {
        return CreateAsync<Rental>("/api/v2/rentals", new RentalRequest(carId, customerId, rentDate ?? RentDate, returnDate ?? ReturnDate));
    }
}
