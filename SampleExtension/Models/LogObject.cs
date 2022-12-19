using System.Text.Json.Serialization;

public sealed class LogObject
{
    [JsonPropertyName("time")]
    public DateTime Timestamp { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("record")]
    [JsonConverter(typeof(InfoToStringConverter))]
    public string LogMessage { get; set; }
}