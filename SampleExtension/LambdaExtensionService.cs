using SampleExtension;

public class LambdaExtensionService : IHostedService
{
    private readonly ILogger<LambdaExtensionService> _log;
    private readonly LambdaExtensionClient _lambdaExtensionClient;

    public LambdaExtensionService(
        ILogger<LambdaExtensionService> log,
        LambdaExtensionClient lambdaExtensionClient)
    {
        _log = log;
        _lambdaExtensionClient = lambdaExtensionClient;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _lambdaExtensionClient.RegisterAsync();
        await _lambdaExtensionClient.SubscribeToLogs();
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var res = await _lambdaExtensionClient.NextAsync();
                if (res == null) continue;
                if (res.EventType == "SHUTDOWN") break;
            }
            catch (Exception e)
            {
                _log.LogWarning(e, "Caught Exception");
                Console.WriteLine(e);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}