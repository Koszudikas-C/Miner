using LibCommunicationStatus;
using LibCryptography.Interface;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.Interface;
using WorkClientBlockChain.Middleware.Interface;

namespace WorkClientBlockChain.Service;

public class WorkService(ILogger<WorkService> logger, IConnectionAndAuth connectionAndAuth,
    IClientContext clientContext, IConnectionMiddleware connectionMiddleware, ICryptographFile cryptograph) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cts)
    {
        await connectionAndAuth.ConnectAndAuthAsync(cts);

       _ = Task.Run(async () => 
       await connectionMiddleware.MonitoringConnectionWorkAsync(cts), cts).ConfigureAwait(false);
        
       var i = 0;
        while (!cts.IsCancellationRequested)
        {
            if (!CommunicationStatus.IsConnected)
            {
               await Task.Delay(1000, cts);
                continue;
            }
            
            // if (logger.IsEnabled(LogLevel.Information))
            // {
            //     logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            // }
            
            // if (i++ == 5) CommunicationStatus.SetConnected(false);
            
            await Task.Delay(1000, cts);
        }
    }
}