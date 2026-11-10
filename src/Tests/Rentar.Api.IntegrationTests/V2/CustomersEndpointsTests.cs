using Rentar.Modules.Customers.Application;
using Rentar.Modules.Customers.Domain;
using Rentar.Modules.Customers.Presentation.Endpoints.V2;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests.V2;

public class CustomersEndpointsTests(RentarApiFactory factory) : V2TestBase(factory)
{
    [Fact]
    public async Task Create_AnswersCreated_WithTheCustomerAndWhereItLives()
    {
        var response = await Client.PostJsonAsync("/api/v2/customers", new CustomerRequest(7, "Rentar Logistics"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var customer = await response.ReadAsAsync<Customer>();
        Assert.Equal($"/api/v2/customers/{customer.Id}", response.Headers.Location?.OriginalString);

        var fetched = await (await Client.GetJsonAsync($"/api/v2/customers/{customer.Id}")).ReadAsAsync<Customer>();
        Assert.Equal(7, fetched.UserId);
        Assert.Equal("Rentar Logistics", fetched.CompanyName);
    }

    [Fact]
    public async Task GetAll_FiltersByUser()
    {
        FreezeClockAtNoon();
        await CreateAsync<Customer>("/api/v2/customers", new CustomerRequest(1, "First Company"));
        await CreateAsync<Customer>("/api/v2/customers", new CustomerRequest(2, "Second Company"));
        await CreateAsync<Customer>("/api/v2/customers", new CustomerRequest(2, "Third Company"));

        Assert.Equal(["First Company", "Second Company", "Third Company"], await CompanyNamesAsync("/api/v2/customers"));
        Assert.Equal(["Second Company", "Third Company"], await CompanyNamesAsync("/api/v2/customers?userId=2"));
    }

    [Fact]
    public async Task GetAll_DuringTheMaintenanceHour_IsServiceUnavailable()
    {
        FreezeClockInMaintenanceHour();

        var response = await Client.GetJsonAsync("/api/v2/customers");

        await AssertProblemAsync(response, HttpStatusCode.ServiceUnavailable, Messages.MaintenanceTime);
    }

    [Fact]
    public async Task Create_WithCompanyNameShorterThanFiveCharacters_ReturnsValidationProblem()
    {
        var response = await Client.PostJsonAsync("/api/v2/customers", new CustomerRequest(1, "Acme"));

        await AssertValidationProblemAsync(response, "companyName");
    }

    [Fact]
    public async Task UnknownCustomer_IsNotFound()
    {
        var response = await Client.GetJsonAsync("/api/v2/customers/404");

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "Customer 404 was not found.");
    }

    private async Task<List<string>> CompanyNamesAsync(string url)
    {
        var response = await Client.GetJsonAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var customers = await response.ReadAsAsync<List<Customer>>();
        return customers.Select(c => c.CompanyName).Order().ToList();
    }
}
