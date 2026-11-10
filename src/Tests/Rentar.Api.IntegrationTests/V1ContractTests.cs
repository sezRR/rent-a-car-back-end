using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests;

/// <summary>
/// Pins what every v1 endpoint answers - status, <c>success</c>, <c>message</c> and the shape of <c>data</c> - so v1
/// stays exactly as the frontend knows it while the code behind it changes.
/// </summary>
public class V1ContractTests(RentarApiFactory factory) : IntegrationTestBase(factory)
{
    private static readonly byte[] PngBytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00];

    private readonly List<string> _responses = [];

    [Fact]
    public async Task EveryEndpoint_AnswersAsItAlwaysHas()
    {
        Factory.Clock.FreezeAt(new DateTimeOffset(DateTime.Today.AddHours(12)));

        await AuthAndUsersAsync();
        await MaintenanceHourAsync();
        await BrandsColorsAndCarsAsync();
        await CarImagesAsync();
        await CustomersRentalsPaymentsAndFindeksAsync();
        await DeletesAsync();

        Assert.Equal(Expected, _responses);
    }

    private static readonly string[] Expected =
    [
        "auth/register => 200 {data: {expiration: string, token: string}, message: \"Access Token Created\", success: true}",
        "auth/register taken => 400 {message: \"User already registered\", success: false}",
        "auth/login => 200 {data: {expiration: string, token: string}, message: \"Access Token Created\", success: true}",
        "auth/login wrong password => 400 \"Password is wrong\"",
        "auth/login unknown email => 400 \"User not found\"",
        "auth/checkpassword => 200 {data: {email: string, findeksRating: number, firstName: string, id: number, lastName: string, passwordHash: string, passwordSalt: string, status: bool}, message: \"Logged in to system successfully\", success: true}",
        "auth/checkpassword wrong password => 400 \"Password is wrong\"",
        "users/getall => 200 {data: [{email: string, findeksRating: number, firstName: string, id: number, lastName: string, passwordHash: string, passwordSalt: string, status: bool}], message: \"Users Listed\", success: true}",
        "users/getbymail => 200 {data: {email: string, findeksRating: number, firstName: string, id: number, lastName: string, passwordHash: string, passwordSalt: string, status: bool}, message: null, success: true}",
        "users/getuserfindeksrating => 200 {data: [{email: string, findeksRating: number, firstName: string, id: number, lastName: string, passwordHash: string, passwordSalt: string, status: bool}], message: null, success: true}",
        "users/getuserbyid => 200 {data: {email: string, findeksRating: number, firstName: string, id: number, lastName: string, passwordHash: string, passwordSalt: string, status: bool}, message: null, success: true}",
        "users/add => 200 {message: \"User Added\", success: true}",
        "users/update => 200 {data: {email: string, findeksRating: number, firstName: string, id: number, lastName: string, passwordHash: string, passwordSalt: string, status: bool}, message: \"Your settings successfully updated\", success: true}",
        "users/getall maintenance => 400 {data: null, message: \"Servers in Maintenance (Time)\", success: false}",
        "customers/getall maintenance => 400 {data: null, message: \"Servers in Maintenance (Time)\", success: false}",
        "rentals/getrentaldetails maintenance => 400 {data: null, message: \"Servers in Maintenance (Time)\", success: false}",
        "brands/add => 200 {message: \"Brand Added\", success: true}",
        "brands/add invalid => 400 {Errors: [{AttemptedValue: string, CustomState: null, ErrorCode: string, ErrorMessage: string, FormattedMessagePlaceholderValues: object, PropertyName: string, Severity: number}], Message: string, StatusCode: 400}",
        "brands/getall => 200 {data: [{brandName: string, id: number}], message: \"Brands Listed\", success: true}",
        "brands/getbyid => 200 {data: {brandName: string, id: number}, message: null, success: true}",
        "brands/update => 200 {message: \"Brand Updated\", success: true}",
        "colors/add => 200 {message: \"Color Added\", success: true}",
        "colors/getall => 200 {data: [{colorName: string, id: number}], message: \"Colors Listed\", success: true}",
        "colors/getbyid => 200 {data: {colorName: string, id: number}, message: null, success: true}",
        "colors/update => 200 {message: \"Color Updated\", success: true}",
        "colors/update invalid => 400 {message: \"Color Name is Invalid\", success: false}",
        "cars/add => 200 {message: \"Car Added\", success: true}",
        "cars/addreturnabledata => 200 {data: {brandId: number, colorId: number, dailyPrice: number, description: string, id: number, minimumFindeksRating: number, modelYear: number}, message: null, success: true}",
        "cars/getall => 200 {data: [{brandId: number, colorId: number, dailyPrice: number, description: string, id: number, minimumFindeksRating: number, modelYear: number}], message: \"Cars Listed\", success: true}",
        "cars/getcardetails => 200 {data: [{brandId: number, brandName: string, carId: number, colorId: number, colorName: string, dailyPrice: number, description: string, id: number, imagePath: null, minimumFindeksRating: number, modelYear: number}], message: null, success: true}",
        "cars/getcardetailsbyid => 200 {data: {brandId: number, brandName: string, carId: number, colorId: number, colorName: string, dailyPrice: number, description: string, id: number, imagePath: null, minimumFindeksRating: number, modelYear: number}, message: null, success: true}",
        "cars/getcarsbybrandid => 200 {data: [{brandId: number, brandName: string, carId: number, colorId: number, colorName: string, dailyPrice: number, description: string, id: number, imagePath: null, minimumFindeksRating: number, modelYear: number}], message: null, success: true}",
        "cars/getcarsbycolorid => 200 {data: [{brandId: number, brandName: string, carId: number, colorId: number, colorName: string, dailyPrice: number, description: string, id: number, imagePath: null, minimumFindeksRating: number, modelYear: number}], message: \"Cars Listed\", success: true}",
        "cars/getcarsbybrandidandcolorid => 200 {data: [{brandId: number, brandName: string, carId: number, colorId: number, colorName: string, dailyPrice: number, description: string, id: number, imagePath: null, minimumFindeksRating: number, modelYear: number}], message: null, success: true}",
        "cars/update => 200 {message: \"Car Updated\", success: true}",
        "carimages/add without file => 200 {message: null, success: true}",
        "carimages/add invalid file => 400 {message: \"Only PNG and JPEG images (.png, .jpg, .jpeg) can be uploaded\", success: false}",
        "carimages/getall => 200 {data: [{carId: number, date: string, id: number, imagePath: string}], message: null, success: true}",
        "carimages/getbyid => 200 {data: {carId: number, date: string, id: number, imagePath: string}, message: null, success: true}",
        "carimages/getbycarid => 200 {data: [{carId: number, date: string, id: number, imagePath: string}], message: null, success: true}",
        "carimages/update => 200 {message: null, success: true}",
        "carimages/update unknown image => 400 {message: \"Car image not found\", success: false}",
        "carimages/delete => 200 {message: null, success: true}",
        "carimages/delete without images => 400 \"This car do not have image!\"",
        "customers/add => 200 {message: \"Customer Added\", success: true}",
        "customers/getall => 200 {data: [{companyName: string, id: number, userId: number}], message: \"Customers Listed\", success: true}",
        "customers/getcustomersbyuserid => 200 {data: [{companyName: string, id: number, userId: number}], message: null, success: true}",
        "rentals/add => 200 {message: \"Rental Added\", success: true}",
        "rentals/getall => 200 {data: [{carId: number, customerId: number, id: number, rentDate: string, returnDate: string}], message: \"Rentals Listed\", success: true}",
        "rentals/getrentalbycarid => 200 {data: {carId: number, customerId: number, id: number, rentDate: string, returnDate: string}, message: null, success: true}",
        "rentals/getrentaldetails => 200 {data: [{brandName: string, carId: number, colorName: string, companyName: string, dailyPrice: number, description: string, modelYear: number, rentDate: string, rentalId: number, returnDate: string, userName: string}], message: null, success: true}",
        "rentals/isrentable => 200 {message: null, success: true}",
        "rentals/isrentable taken => 400 {message: \"This car already rentaled by someone\", success: false}",
        "payments/pay => 200 {message: \"Payment successful\", success: true}",
        "payments/pay => 400 {message: \"Payment failed\", success: false}",
        "findeks/calculate => 200 {data: number, message: \"Findeks rating calculated successfully\", success: true}",
        "cars/delete => 200 {message: \"Car Deleted\", success: true}",
        "cars/delete unknown car => 500 {Message: string, StatusCode: 500}",
        "colors/delete => 200 {message: \"Color Deleted\", success: true}",
        "brands/delete => 200 {message: \"Brand Deleted\", success: true}",
    ];

    private async Task AuthAndUsersAsync()
    {
        var registration = new { firstName = "Sezer", lastName = "Tetik", email = "driver@example.com", password = "Passw0rd!", findeksRating = 900 };
        await Post("auth/register", "/api/v1/auth/register", registration);
        await Post("auth/register taken", "/api/v1/auth/register", registration);

        await Post("auth/login", "/api/v1/auth/login", new { email = "driver@example.com", password = "Passw0rd!" });
        await Post("auth/login wrong password", "/api/v1/auth/login", new { email = "driver@example.com", password = "wrong" });
        await Post("auth/login unknown email", "/api/v1/auth/login", new { email = "nobody@example.com", password = "Passw0rd!" });
        await Post("auth/checkpassword", "/api/v1/auth/checkpassword", new { email = "driver@example.com", password = "Passw0rd!" });
        await Post("auth/checkpassword wrong password", "/api/v1/auth/checkpassword", new { email = "driver@example.com", password = "wrong" });

        await Get("users/getall", "/api/v1/users/getall");
        await Get("users/getbymail", "/api/v1/users/getbymail?email=driver@example.com");
        await Get("users/getuserfindeksrating", "/api/v1/users/getuserfindeksrating?findeksRating=500");
        await Get("users/getuserbyid", "/api/v1/users/getuserbyid?id=1");
        await Post("users/add", "/api/v1/users/add", new { firstName = "Ada", lastName = "Lovelace", email = "ada@example.com", passwordHash = "AAAA", passwordSalt = "AAAA", status = true, findeksRating = 1200 });
        await Post("users/update", "/api/v1/users/update", new { id = 1, firstName = "Sezer", lastName = "Tetik", email = "driver@example.com", password = "Passw0rd!", findeksRating = 1000 });
    }

    private async Task MaintenanceHourAsync()
    {
        Factory.Clock.FreezeAt(new DateTimeOffset(DateTime.Today.AddHours(22).AddMinutes(30)));

        await Get("users/getall maintenance", "/api/v1/users/getall");
        await Get("customers/getall maintenance", "/api/v1/customers/getall");
        await Get("rentals/getrentaldetails maintenance", "/api/v1/rentals/getrentaldetails");

        Factory.Clock.FreezeAt(new DateTimeOffset(DateTime.Today.AddHours(12)));
    }

    private async Task BrandsColorsAndCarsAsync()
    {
        await Post("brands/add", "/api/v1/brands/add", new { brandName = "Toyota" });
        await Post("brands/add invalid", "/api/v1/brands/add", new { brandName = "A" });
        await Get("brands/getall", "/api/v1/brands/getall");
        await Get("brands/getbyid", "/api/v1/brands/getbyid?id=1");
        await Post("brands/update", "/api/v1/brands/update", new { id = 1, brandName = "Toyota" });

        await Post("colors/add", "/api/v1/colors/add", new { colorName = "Red" });
        await Get("colors/getall", "/api/v1/colors/getall");
        await Get("colors/getbyid", "/api/v1/colors/getbyid?id=1");
        await Post("colors/update", "/api/v1/colors/update", new { id = 1, colorName = "Red" });
        await Post("colors/update invalid", "/api/v1/colors/update", new { id = 1, colorName = "R" });

        var car = new { brandId = 1, colorId = 1, modelYear = 2021, dailyPrice = 950m, description = "Corolla 1.6", minimumFindeksRating = 700 };
        await Post("cars/add", "/api/v1/cars/add", car);
        await Post("cars/addreturnabledata", "/api/v1/cars/addreturnabledata", car with { description = "Civic 1.5" });
        await Get("cars/getall", "/api/v1/cars/getall");
        await Get("cars/getcardetails", "/api/v1/cars/getcardetails");
        await Get("cars/getcardetailsbyid", "/api/v1/cars/getcardetailsbyid?id=1");
        await Get("cars/getcarsbybrandid", "/api/v1/cars/getcarsbybrandid?brandId=1");
        await Get("cars/getcarsbycolorid", "/api/v1/cars/getcarsbycolorid?colorId=1");
        await Get("cars/getcarsbybrandidandcolorid", "/api/v1/cars/getcarsbybrandidandcolorid?brandId=1&colorId=1");
        await Post("cars/update", "/api/v1/cars/update", new { id = 2, brandId = 1, colorId = 1, modelYear = 2022, dailyPrice = 990m, description = "Civic 1.5", minimumFindeksRating = 700 });
    }

    private async Task CarImagesAsync()
    {
        await Form("carimages/add without file", HttpMethod.Post, "/api/v1/carimages/add", ("carId", "1"));
        await Form("carimages/add invalid file", HttpMethod.Post, "/api/v1/carimages/add", ("carId", "1"), ("file", "notes.md", Encoding.UTF8.GetBytes("# notes")));
        await Get("carimages/getall", "/api/v1/carimages/getall");
        await Get("carimages/getbyid", "/api/v1/carimages/getbyid?id=1");
        await Get("carimages/getbycarid", "/api/v1/carimages/getbycarid?carId=1");
        await Form("carimages/update", HttpMethod.Post, "/api/v1/carimages/update", ("Id", "1"), ("CarId", "1"), ("objectFile", "car.png", PngBytes));
        await Form("carimages/update unknown image", HttpMethod.Post, "/api/v1/carimages/update", ("Id", "999"), ("CarId", "1"), ("objectFile", "car.png", PngBytes));
        await Send("carimages/delete", HttpMethod.Delete, "/api/v1/carimages/delete?id=1");
        await Send("carimages/delete without images", HttpMethod.Delete, "/api/v1/carimages/delete?id=1");
    }

    private async Task CustomersRentalsPaymentsAndFindeksAsync()
    {
        await Post("customers/add", "/api/v1/customers/add", new { userId = 1, companyName = "Rentar Logistics" });
        await Get("customers/getall", "/api/v1/customers/getall");
        await Get("customers/getcustomersbyuserid", "/api/v1/customers/getcustomersbyuserid?userId=1");

        await Post("rentals/add", "/api/v1/rentals/add", new { carId = 1, customerId = 1, rentDate = "2026-09-18T10:00:00", returnDate = "2026-09-20T10:00:00" });
        await Get("rentals/getall", "/api/v1/rentals/getall");
        await Get("rentals/getrentalbycarid", "/api/v1/rentals/getrentalbycarid?carId=1");
        await Get("rentals/getrentaldetails", "/api/v1/rentals/getrentaldetails");
        await Post("rentals/isrentable", "/api/v1/rentals/isrentable", new { carId = 1, customerId = 1, rentDate = "2026-09-25T10:00:00", returnDate = "2026-09-26T10:00:00" });
        await Post("rentals/isrentable taken", "/api/v1/rentals/isrentable", new { carId = 1, customerId = 1, rentDate = "2026-09-19T10:00:00", returnDate = "2026-09-21T10:00:00" });

        // The payment outcome is random, so both answers are collected and recorded once each.
        var payments = new SortedSet<string>(StringComparer.Ordinal);
        for (var attempt = 0; attempt < 64 && payments.Count < 2; attempt++)
        {
            var response = await Client.GetJsonAsync("/api/v1/payments/pay");
            payments.Add(await DescribeAsync("payments/pay", response));
        }
        _responses.AddRange(payments);

        await Get("findeks/calculate", "/api/v1/findeks/calculate");
    }

    private async Task DeletesAsync()
    {
        await Post("cars/delete", "/api/v1/cars/delete", new { id = 2 });
        await Post("cars/delete unknown car", "/api/v1/cars/delete", new { id = 999 });
        await Post("colors/delete", "/api/v1/colors/delete", new { id = 1, colorName = "Red" });
        await Post("brands/delete", "/api/v1/brands/delete", new { id = 1, brandName = "Toyota" });
    }

    private async Task Get(string name, string url)
    {
        _responses.Add(await DescribeAsync(name, await Client.GetJsonAsync(url)));
    }

    private async Task Post(string name, string url, object body)
    {
        _responses.Add(await DescribeAsync(name, await Client.PostJsonAsync(url, body)));
    }

    private async Task Send(string name, HttpMethod method, string url)
    {
        using var request = new HttpRequestMessage(method, url);
        _responses.Add(await DescribeAsync(name, await Client.SendAsync(request, TestContext.Current.CancellationToken)));
    }

    private async Task Form(string name, HttpMethod method, string url, params object[] fields)
    {
        using var form = new MultipartFormDataContent();
        foreach (var field in fields)
        {
            switch (field)
            {
                case (string key, string value):
                    form.Add(new StringContent(value), key);
                    break;
                case (string key, string fileName, byte[] bytes):
                    form.Add(new ByteArrayContent(bytes), key, fileName);
                    break;
            }
        }

        using var request = new HttpRequestMessage(method, url) { Content = form };
        _responses.Add(await DescribeAsync(name, await Client.SendAsync(request, TestContext.Current.CancellationToken)));
    }

    /// <summary>
    /// One line per response: the status, then the body with its keys sorted. <c>success</c>, <c>message</c> and status
    /// codes keep their values; everything else is reduced to its JSON kind, and objects and arrays to their keys.
    /// </summary>
    private static async Task<string> DescribeAsync(string name, HttpResponseMessage response)
    {
        var body = await response.ReadTextAsync();
        var shape = body.Length == 0 ? "<empty>" : Describe(JsonDocument.Parse(body).RootElement, isRoot: true);
        return $"{name} => {(int)response.StatusCode} {shape}";
    }

    private static readonly HashSet<string> LiteralKeys = new(StringComparer.Ordinal) { "success", "message", "statusCode", "StatusCode" };

    private static string Describe(JsonElement element, bool isRoot = false, int depth = 0)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => depth > 2
                ? "object"
                : "{" + string.Join(", ", element.EnumerateObject()
                    .OrderBy(p => p.Name, StringComparer.Ordinal)
                    .Select(p => $"{p.Name}: {(isRoot && LiteralKeys.Contains(p.Name) ? p.Value.GetRawText() : Describe(p.Value, depth: depth + 1))}")) + "}",
            JsonValueKind.Array => element.GetArrayLength() == 0 ? "[]" : $"[{Describe(element[0], depth: depth + 1)}]",
            JsonValueKind.String => isRoot ? element.GetRawText() : "string",
            JsonValueKind.Number => "number",
            JsonValueKind.True or JsonValueKind.False => "bool",
            _ => "null"
        };
    }
}
