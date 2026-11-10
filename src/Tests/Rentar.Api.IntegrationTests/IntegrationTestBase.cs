using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public abstract class IntegrationTestBase(RentarApiFactory factory) : IAsyncLifetime
{
    protected RentarApiFactory Factory { get; } = factory;

    protected HttpClient Client { get; } = factory.CreateClient();

    public async ValueTask InitializeAsync()
    {
        await Factory.ResetAsync();
    }

    public ValueTask DisposeAsync()
    {
        Client.Dispose();
        return ValueTask.CompletedTask;
    }
}
