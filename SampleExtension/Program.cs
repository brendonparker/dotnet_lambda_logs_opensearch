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

app.MapPut("/lambda_logs", Handle);
app.MapPost("/lambda_logs", Handle);

await app.RunAsync("http://sandbox:4243");

static async Task Handle(HttpContext context, [FromServices] ILogger<LambdaExtensionClient> log)
{
    var sr = new StreamReader(context.Request.Body);
    var content = await sr.ReadToEndAsync();
    log.LogInformation("Received Logs! {HttpMethod}", context.Request.Method);
    log.LogInformation(content);
}