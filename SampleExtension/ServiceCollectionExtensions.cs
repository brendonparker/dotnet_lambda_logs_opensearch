namespace SampleExtension;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExtensionClient(this IServiceCollection services)
    {
        services.AddSingleton<LambdaExtensionClient>();
        services.AddHttpClient(LambdaExtensionClient.EXTENSION_CLIENT)
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress =
                    new Uri($"http://{Environment.GetEnvironmentVariable("AWS_LAMBDA_RUNTIME_API")}");
            });
        return services;
    }
}