using LibCommunicationStateClient.Entities;
using LibEntitiesClient.Entities;
using LibHandlerClient.Entities;
using WorkClientBlockChain.Interface;
using WorkClientBlockChain.Utils.Interface;

namespace WorkClientBlockChain.Service;

public class WorkService(ILogger<WorkService> logger, IConnectionAndAuth connectionAndAuth,
    IPosAuth posAuth) : BackgroundService
{
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;
    protected override async Task ExecuteAsync(CancellationToken cts)
    {
        try
        {
            var task = new TaskCompletionSource<ClientInfo>();

            void Handler(ClientInfo clientInfo)
            {
                task.TrySetResult(clientInfo);
                posAuth.SendClientMine(clientInfo);
                posAuth.ReceiveDataCrypt(clientInfo);
            }

            _globalEventBus.Subscribe<ClientInfo>(Handler);

            connectionAndAuth.ConnectAndAuthAsync(cts).GetAwaiter();

            // while (!cts.IsCancellationRequested)
            // {
            //
            //     // if (logger.IsEnabled(LogLevel.Information))
            //     // {
            //     //     logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //     // }
            //
            //     // if (i++ == 5) CommunicationStateReceiveAndSend.SetConnected(false);
            //
            //     await Task.Delay(1000, cts);
            // }
        }
        catch (Exception)
        {
            throw new Exception();
        }
    }
}
