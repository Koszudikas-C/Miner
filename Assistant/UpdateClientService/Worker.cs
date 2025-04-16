using LibCommunicationStatus;
using LibSsl.Interface;
using UpdateClientService.Connection;
using UpdateClientService.Interface;

namespace UpdateClientService;

public class Worker(ILogger<Worker> logger, IConnectionAndAuth connectionAndAuth
    ,IAuthSsl authSsl) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cts)
    {
        await connectionAndAuth.ConnectAndAuthAsync(cts);
        
        while (!cts.IsCancellationRequested)
        {
            if (CommunicationStatus.IsSending)
            {
               await Task.Delay(1000, cts);
                continue;
            }
            
            if (ClientContext.Instance.ClientInfo() == null) continue;
            
            if (!ClientContext.Instance.ClientInfo()!.Socket!.Connected) continue;
            
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, cts);
        }
    }
}