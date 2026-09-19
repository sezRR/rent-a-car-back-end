using Core.Extensions;
using Core.Utilities.Results;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace WebAPI.IntegrationTests
{
    public class RentalsEndpointsTests : IntegrationTestBase
    {
        private static readonly DateTime RentDate = new DateTime(2026, 9, 18, 10, 0, 0, DateTimeKind.Unspecified);
        private static readonly DateTime ReturnDate = new DateTime(2026, 9, 20, 10, 0, 0, DateTimeKind.Unspecified);

        public RentalsEndpointsTests(RentarApiFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Add_StoresLocalDateTimes_AndGetAllReturnsThem()
        {
            var response = await Client.PostJsonAsync("/api/rentals/add", NewRental());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var rentals = await Client.GetJsonAsync("/api/rentals/getall");
            var listed = await rentals.ReadAsAsync<DataResult<List<Rental>>>();

            var rental = Assert.Single(listed.Data);
            Assert.Equal(RentDate, rental.RentDate);
            Assert.Equal(ReturnDate, rental.ReturnDate);
            Assert.Equal(1, rental.CarId);
        }

        [Fact]
        public async Task GetRentalByCarId_ReturnsRentalOfThatCar()
        {
            await Client.PostJsonAsync("/api/rentals/add", NewRental(carId: 1));
            await Client.PostJsonAsync("/api/rentals/add", NewRental(carId: 2, customerId: 7));

            var response = await Client.GetJsonAsync("/api/rentals/getrentalbycarid?carId=2");
            var result = await response.ReadAsAsync<DataResult<Rental>>();

            Assert.Equal(2, result.Data.CarId);
            Assert.Equal(7, result.Data.CustomerId);
        }

        [Fact]
        public async Task Add_WithoutDates_ReturnsValidationErrors()
        {
            var response = await Client.PostJsonAsync("/api/rentals/add", new Rental
            {
                CarId = 1,
                CustomerId = 1
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var error = await response.ReadAsAsync<ValidationErrorDetails>();
            Assert.Contains(error.Errors, e => e.PropertyName == "RentDate");
            Assert.Contains(error.Errors, e => e.PropertyName == "ReturnDate");
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
}
