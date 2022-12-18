using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using SampleExtension;
using Serilog;
using Serilog.Formatting.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, config) =>
{
    config.WriteTo.Console(new ElasticsearchJsonFormatter());
});

builder.Services.AddExtensionClient();
builder.Services.AddHostedService<LambdaExtensionService>();

var app = builder.Build();

app.MapPost("/lambda_logs", HandlePost);

app.Run("http://sandbox:4243");

static async Task HandlePost(
    [FromServices] ILogger<LambdaExtensionClient> log,
    [FromBody] List<LogObject> logs)
{
    log.LogInformation("Received Logs!");
}

public sealed class LogObject
{
    [JsonPropertyName("time")]
    public DateTime Timestamp { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("record")]
    public string LogMessage { get; set; }
}