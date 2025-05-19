
using LibClassManagerOptions.Interface;
using LibClassProcessOperations.Entities;
using LibCommunicationStatus;
using LibCryptography.Interface;
using LibHandler.EventBus;
using LibRemoteAndClient.Entities.Remote.Client;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.Interface;
using WorkClientBlockChain.Middleware.Interface;
using WorkClientBlockChain.Utils.Interface;

namespace WorkClientBlockChain.Service;

public class WorkService(ILogger<WorkService> logger, IConnectionAndAuth connectionAndAuth,
    IClientConnected clientConnected, IConnectionMiddleware connectionMiddleware, ICryptographFile cryptograph,
    IPosAuth posAuth, IManagerOptions<ParamsSocks5> managerOptions) : BackgroundService
{
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;
    protected override async Task ExecuteAsync(CancellationToken cts)
    {
        var task = new TaskCompletionSource<ClientInfo>();

        void Handler(ClientInfo clientInfo)
        {
            task.TrySetResult(clientInfo);
            posAuth.SendClientMine(clientInfo);
            posAuth.ReceiveDataCrypt(clientInfo);
        }
        
        _globalEventBusClient.Subscribe<ClientInfo>(Handler);
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