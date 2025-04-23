using LibCommunicationStatus;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Interface;
using WorkClientBlockChain.Connection;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.Interface;
using WorkClientBlockChain.Middleware.Interface;

namespace WorkClientBlockChain.Service;

public class WorkService(ILogger<WorkService> logger, IConnectionAndAuth connectionAndAuth,
    IClientContext clientContext, IConnectionMiddleware connectionMiddleware) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cts)
    {

        var config = new ConnectionConfig() { Port = 5051, MaxConnections = 0 };
        await connectionAndAuth.ConnectAndAuthAsync(config, cts);

       _ = Task.Run(async () => 
       await connectionMiddleware.MonitoringConnectionWorkAsync(cts)).ConfigureAwait(false);
        
        while (!cts.IsCancellationRequested)
        {
            if (CommunicationStatus.IsSending)
            {
               await Task.Delay(1000, cts);
                continue;
            }
            
            if (clientContext.GetClientInfo() == null) continue;
            
            if (!clientContext.GetClientInfo()!.SocketWrapper!.Connected) continue;
            
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, cts);
        }
    }
}