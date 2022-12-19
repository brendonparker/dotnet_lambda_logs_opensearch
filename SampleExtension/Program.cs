using System.Text;
using System.Text.Json.Serialization;
using Amazon.KinesisFirehose;
using Amazon.KinesisFirehose.Model;
using Microsoft.AspNetCore.Mvc;
using SampleExtension;
using Serilog;
using Serilog.Formatting.Elasticsearch;

var DELIVERY_STREAM_NAME = Environment.GetEnvironmentVariable("DELIVERY_STREAM_NAME");

if (string.IsNullOrWhiteSpace(DELIVERY_STREAM_NAME))
{
    Console.WriteLine("DELIVERY_STREAM_NAME environment variable not yet. Extension will not run.");
    return;
}

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, config) => { config.WriteTo.Console(new ElasticsearchJsonFormatter()); });

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonKinesisFirehose>();
builder.Services.AddExtensionClient();
builder.Services.AddHostedService<LambdaExtensionBackgroundService>();

var app = builder.Build();

app.Map("/lambda_logs", async (
    HttpContext context,
    [FromServices] ILogger<LambdaExtensionClient> log,
    [FromServices] IAmazonKinesisFirehose firehose,
    [FromBody] List<LogObject> logs) =>
{
    log.LogInformation("Received Logs...");
    try
    {
        var res = await firehose.PutRecordBatchAsync(new PutRecordBatchRequest
        {
            DeliveryStreamName = DELIVERY_STREAM_NAME,
            Records = logs.ConvertAll(x => new Record { Data = new MemoryStream(Encoding.UTF8.GetBytes(x.LogMessage)) })
        });
        log.LogInformation("PutRecordBatchAsync {HttpStatusCode} {FailedPutCount} {TotalCount}", res.HttpStatusCode,
            res.FailedPutCount, logs.Count);
    }
    catch (Exception e)
    {
        log.LogError(e, "Caught Exception");
    }
});

app.Run("http://sandbox:4243");

public sealed class LogObject
{
    [JsonPropertyName("time")] public DateTime Timestamp { get; set; }
    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("record")]
    [JsonConverter(typeof(InfoToStringConverter))]
    public string LogMessage { get; set; }
}