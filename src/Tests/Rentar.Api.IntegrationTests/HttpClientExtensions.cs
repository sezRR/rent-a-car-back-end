using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests;

/// <summary>
/// Thin wrappers that hand the ambient test cancellation token to every HTTP call.
/// </summary>
public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions Options = CreateOptions();

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return options;
    }

    public static Task<HttpResponseMessage> GetJsonAsync(this HttpClient client, string url)
    {
        return client.GetAsync(url, TestContext.Current.CancellationToken);
    }

    public static Task<HttpResponseMessage> PostJsonAsync(this HttpClient client, string url, object body)
    {
        return client.PostAsJsonAsync(url, body, TestContext.Current.CancellationToken);
    }

    public static Task<HttpResponseMessage> PutJsonAsync(this HttpClient client, string url, object body)
    {
        return client.PutAsJsonAsync(url, body, TestContext.Current.CancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteResourceAsync(this HttpClient client, string url)
    {
        return client.DeleteAsync(url, TestContext.Current.CancellationToken);
    }

    public static async Task<T> ReadAsAsync<T>(this HttpResponseMessage response)
    {
        var json = await response.ReadTextAsync();
        return JsonSerializer.Deserialize<T>(json, Options);
    }

    public static Task<string> ReadTextAsync(this HttpResponseMessage response)
    {
        return response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
    }
}
