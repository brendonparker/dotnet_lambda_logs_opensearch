using System.Text;
using System.Text.Json;
using SampleExtension.Models;
using static Constants;

namespace SampleExtension;

public class LambdaExtensionClient
{
    private readonly ILogger<LambdaExtensionClient> _log;
    private readonly IHttpClientFactory _httpClientFactory;
    public const string EXTENSION_CLIENT = "ExtensionClient";
    private static string LambdaExtensionIdentifier = null;

    public LambdaExtensionClient(
        ILogger<LambdaExtensionClient> log,
        IHttpClientFactory httpClientFactory)
    {
        _log = log;
        _httpClientFactory = httpClientFactory;
    }

    public async Task RegisterAsync()
    {
        _log.LogInformation("Registering...");

        using var client = _httpClientFactory.CreateClient(EXTENSION_CLIENT);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/2020-01-01/extension/register");
        httpRequestMessage.Headers.Add(LAMBDA_EXTENSION_NAME, "sample-extension");
        var registerRequest = new
        {
            events = new[]
            {
                EVENT_TYPE_INVOKE, EVENT_TYPE_SHUTDOWN
            }
        };
        httpRequestMessage.Content = JsonContent(registerRequest);
        var res = await client.SendAsync(httpRequestMessage);
        _log.LogInformation("Response: {ResponseBody}", await res.Content.ReadAsStringAsync());
        if (res.Headers.TryGetValues(LAMBDA_EXTENSION_IDENTIFIER, out var values))
        {
            LambdaExtensionIdentifier = values.First();
        }
    }

    public async Task SubscribeToLogs()
    {
        if (string.IsNullOrWhiteSpace(LambdaExtensionIdentifier)) return;

        _log.LogInformation("Subscribing to Logs...");

        using var client = _httpClientFactory.CreateClient(EXTENSION_CLIENT);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, "/2020-08-15/logs");
        httpRequestMessage.Headers.Add(LAMBDA_EXTENSION_IDENTIFIER, LambdaExtensionIdentifier);
        var subscriptionRequest = new ExtensionSubscriptionRequest
        {
            SchemaVersion = "2020-08-15",
            Types = new[] { LOGS_FUNCTION },
            Buffering = new BufferingParameters
            {
                MaxItems = 1000,
                MaxBytes = 5 * 1024 * 1024,
                TimeoutInMilliseconds = 100
            },
            Destination = new SubscriptionDestination
            {
                Protocol = PROTOCOL_HTTP,
                Uri = $"{ENDPOINT}/lambda_logs"
            }
        };
        httpRequestMessage.Content = JsonContent(subscriptionRequest);
        var res = await client.SendAsync(httpRequestMessage);
        _log.LogInformation("Response: {ResponseBody}", await res.Content.ReadAsStringAsync());
    }

    public async Task<NextEventResponse> NextAsync()
    {
        if (string.IsNullOrWhiteSpace(LambdaExtensionIdentifier)) return null;

        using var client = _httpClientFactory.CreateClient(EXTENSION_CLIENT);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "/2020-01-01/extension/event/next");
        httpRequestMessage.Headers.Add(LAMBDA_EXTENSION_IDENTIFIER, LambdaExtensionIdentifier);
        var res = await client.SendAsync(httpRequestMessage);
        _log.LogInformation("Response: {ResponseBody}", await res.Content.ReadAsStringAsync());
        return await res.Content.ReadFromJsonAsync<NextEventResponse>();
    }

    private StringContent JsonContent<T>(T body) =>
        new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
}