using System.Text.Json.Serialization;

namespace SampleExtension.Models;

public sealed class ExtensionSubscriptionRequest
{
    [JsonPropertyName("schemaVersion")] public string SchemaVersion { get; set; }

    /// <summary>
    /// Valid values: platform, function
    /// </summary>
    [JsonPropertyName("types")]
    public string[] Types { get; set; }

    [JsonPropertyName("buffering")] public BufferingParameters Buffering { get; set; }
    [JsonPropertyName("destination")] public SubscriptionDestination Destination { get; set; }
}

public sealed class BufferingParameters
{
    [JsonPropertyName("maxItems")] public int MaxItems { get; set; }
    [JsonPropertyName("maxBytes")] public int MaxBytes { get; set; }
    [JsonPropertyName("timeoutMs")] public int TimeoutInMilliseconds { get; set; }
}

public sealed class SubscriptionDestination
{
    [JsonPropertyName("protocol")] public string Protocol { get; set; }
    [JsonPropertyName("URI")] public string Uri { get; set; }
}