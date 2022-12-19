using SampleExtension;

public class LambdaExtensionBackgroundService : BackgroundService
{
    private readonly ILogger<LambdaExtensionBackgroundService> _log;
    private readonly LambdaExtensionClient _lambdaExtensionClient;

    public LambdaExtensionBackgroundService(
        ILogger<LambdaExtensionBackgroundService> log,
        LambdaExtensionClient lambdaExtensionClient)
    {
        _log = log;
        _lambdaExtensionClient = lambdaExtensionClient;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
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
}