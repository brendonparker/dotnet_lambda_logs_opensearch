using Serilog;
using Serilog.Formatting.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, config) => { config.WriteTo.Console(new ElasticsearchJsonFormatter()); });

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();