using LibCommunicationStatus;
using LibSocket.Entities;
using LibSsl.Interface;
using WorkClientBlockChain.Connection;
using WorkClientBlockChain.Interface;

namespace WorkClientBlockChain.Service;

public class WorkService(ILogger<WorkService> logger, IConnectionAndAuth connectionAndAuth
    ,IAuthSsl authSsl) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cts)
    {
        var config = new ConnectionConfig() { Port = 5051, MaxConnections = 0 };
        await connectionAndAuth.ConnectAndAuthAsync(config, cts);
        
        while (!cts.IsCancellationRequested)
        {
            if (CommunicationStatus.IsSending)
            {
               await Task.Delay(1000, cts);
                continue;
            }
            
            if (ClientContext.GetClientInfo() == null) continue;
            
            if (!ClientContext.GetClientInfo()!.Socket!.Connected) continue;
            
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, cts);
        }
    }
}