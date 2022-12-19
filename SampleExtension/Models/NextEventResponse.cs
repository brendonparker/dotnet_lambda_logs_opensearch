using System.Text.Json.Serialization;

namespace SampleExtension.Models;

public sealed class NextEventResponse
{
    [JsonPropertyName("eventType")] public string EventType { get; set; }
    [JsonPropertyName("deadlineMs")] public long DeadlineMs { get; set; }
    [JsonPropertyName("requestId")] public string RequestId { get; set; }

    [JsonPropertyName("invokedFunctionArn")]
    public string InvokedFunctionArn { get; set; }

    [JsonPropertyName("tracing")] public Tracing Tracing { get; set; }
}

public sealed class Tracing
{
    [JsonPropertyName("type")] public string Type { get; set; }
    [JsonPropertyName("value")] public string Value { get; set; }
}