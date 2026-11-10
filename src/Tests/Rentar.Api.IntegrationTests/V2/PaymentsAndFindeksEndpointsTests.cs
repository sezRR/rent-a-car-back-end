using Rentar.Modules.Findeks.Presentation.Endpoints.V2;
using Rentar.Modules.Payments.Application;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests.V2;

public class PaymentsAndFindeksEndpointsTests(RentarApiFactory factory) : V2TestBase(factory)
{
    [Fact]
    public async Task Payment_EitherSucceedsWithNoContent_OrIsDeclinedWithPaymentRequired()
    {
        // The payment outcome is simulated at random, so both answers are checked.
        var seenSuccess = false;
        var seenDecline = false;

        for (var attempt = 0; attempt < 64 && !(seenSuccess && seenDecline); attempt++)
        {
            var response = await Client.PostJsonAsync("/api/v2/payments", new { });

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                seenSuccess = true;
            }
            else
            {
                await AssertProblemAsync(response, HttpStatusCode.PaymentRequired, Messages.PaymentFailed);
                seenDecline = true;
            }
        }

        Assert.True(seenSuccess && seenDecline);
    }

    [Fact]
    public async Task FindeksRating_IsReadWithGet()
    {
        var response = await Client.GetJsonAsync("/api/v2/findeks/rating");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var rating = await response.ReadAsAsync<FindeksRatingResponse>();
        Assert.InRange(rating.Rating, 0, 1900);
    }
}
