using LibCommunicationStateClient.Entities;
using LibCryptographyClient.Interface;
using LibEntitiesClient.Entities;
using LibEntitiesClient.Entities.Params;
using LibHandlerClient.Entities;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.Interface;
using WorkClientBlockChain.Middleware.Interface;
using WorkClientBlockChain.Utils.Interface;

namespace WorkClientBlockChain.Service;

public class WorkService(ILogger<WorkService> logger, IConnectionAndAuth connectionAndAuth,
 IConnectionMiddleware connectionMiddleware, IPosAuth posAuth) : BackgroundService
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

            _= Task.Run(async () => await connectionMiddleware.MonitoringConnectionWorkAsync(cts), cts);

            await connectionAndAuth.ConnectAndAuthAsync(cts);

            while (!cts.IsCancellationRequested)
            {
                if (!CommunicationStateReceiveAndSend.IsConnected)
                {
                    await Task.Delay(1000, cts);
                    continue;
                }

                // if (logger.IsEnabled(LogLevel.Information))
                // {
                //     logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                // }

                // if (i++ == 5) CommunicationStateReceiveAndSend.SetConnected(false);

                await Task.Delay(1000, cts);
            }
        }
        catch (Exception)
        {
            throw new Exception();
        }
    }
}
