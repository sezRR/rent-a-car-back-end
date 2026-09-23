using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WebAPI.IntegrationTests;

[Collection(ApiCollection.Name)]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    private readonly RentarApiFactory _factory;

    protected IntegrationTestBase(RentarApiFactory factory)
    {
        _factory = factory;
        Client = factory.CreateClient();
    }

    protected HttpClient Client { get; }

    public async ValueTask InitializeAsync()
    {
        await _factory.ResetAsync();
    }

    public ValueTask DisposeAsync()
    {
        Client.Dispose();
        return ValueTask.CompletedTask;
    }
}
