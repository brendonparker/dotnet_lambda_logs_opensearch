using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SampleExtension;

public class LambdaExtensionClient
{
    private readonly ILogger<LambdaExtensionClient> _log;
    private readonly IHttpClientFactory _httpClientFactory;
    public const string EXTENSION_CLIENT = "ExtensionClient";
    public const string Lambda_Extension_Identifier = "Lambda-Extension-Identifier";
    private string LambdaExtensionIdentifier = null;
    
    public LambdaExtensionClient(
        ILogger<LambdaExtensionClient> log,
        IHttpClientFactory httpClientFactory)
    {
        _log = log;
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task RegisterAsync()
    {
        using var client = _httpClientFactory.CreateClient(EXTENSION_CLIENT);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/2020-01-01/extension/register");
        httpRequestMessage.Headers.Add("Lambda-Extension-Name", "sample-extension");
        var registerRequest = new
        {
            events = new[] { "INVOKE", "SHUTDOWN" }
        };
        httpRequestMessage.Content = new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8,
            "application/json");
        var res = await client.SendAsync(httpRequestMessage);
        _log.LogInformation("Response: {ResponseBody}", await res.Content.ReadAsStringAsync());
        if (res.Headers.TryGetValues(Lambda_Extension_Identifier, out var values))
        {
            LambdaExtensionIdentifier = values.First();
        }
    }
    
    public async Task SubscribeToLogs()
    {
        if (string.IsNullOrWhiteSpace(LambdaExtensionIdentifier)) return;
        
        using var client = _httpClientFactory.CreateClient(EXTENSION_CLIENT);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, "/2020-08-15/logs");
        httpRequestMessage.Headers.Add(Lambda_Extension_Identifier, LambdaExtensionIdentifier);
        var subscriptionRequest = new ExtensionSubscriptionRequest
        {
            SchemaVersion = "2020-08-15",
            Types = new[] { "platform", "function" },
            Buffering = new BufferingParameters
            {
                MaxItems = 1000,
                MaxBytes = 262144,
                TimeoutInMilliseconds = 100
            },
            Destination = new SubscriptionDestination
            {
                Protocol = "HTTP",
                Uri = "http://sandbox:4243/lambda_logs"
            }
        };
        httpRequestMessage.Content = new StringContent(JsonSerializer.Serialize(subscriptionRequest), Encoding.UTF8,
            "application/json");
        var res = await client.SendAsync(httpRequestMessage);
        _log.LogInformation("Response: {ResponseBody}", await res.Content.ReadAsStringAsync());
    }
    
    public async Task<NextEventResponse> NextAsync()
    {
        if (string.IsNullOrWhiteSpace(LambdaExtensionIdentifier)) return null;
        
        using var client = _httpClientFactory.CreateClient(EXTENSION_CLIENT);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "/2020-01-01/extension/event/next");
        httpRequestMessage.Headers.Add(Lambda_Extension_Identifier, LambdaExtensionIdentifier);
        var res = await client.SendAsync(httpRequestMessage);
        _log.LogInformation("Response: {ResponseBody}", await res.Content.ReadAsStringAsync());
        return await res.Content.ReadFromJsonAsync<NextEventResponse>();
    }
}

public sealed class ExtensionSubscriptionRequest
{
    [JsonPropertyName("schemaVersion")]
    public string SchemaVersion { get; set; }
    [JsonPropertyName("types")]
    public string[] Types { get; set; }
    [JsonPropertyName("buffering")]
    public BufferingParameters Buffering { get; set; }
    [JsonPropertyName("destination")]
    public SubscriptionDestination Destination { get; set; }
}

public sealed class BufferingParameters
{
    [JsonPropertyName("maxItems")]
    public int MaxItems { get; set; }
    [JsonPropertyName("maxBytes")]
    public int MaxBytes { get; set; }
    [JsonPropertyName("timeoutMs")]
    public int TimeoutInMilliseconds { get; set; }
}

public sealed class SubscriptionDestination
{
    [JsonPropertyName("protocol")]
    public string Protocol { get; set; }
    [JsonPropertyName("URI")]
    public string Uri { get; set; }
}