using LibCommunicationStateClient.Entities.Enum;
using LibEntitiesClient.Interface;
using LibHandlerClient.Entities;
using LibSocketAndSslStreamClient.Entities.Enum;
using LibSocketAndSslStreamClient.Interface;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.Middleware.Interface;

namespace WorkClientBlockChain.Middleware;

public class ConnectionRemoteState : IConnectionRemoteState
{
    private readonly ISocket _socket;
    private readonly IClientConnected _clientConnected;
    private readonly ILogger<ConnectionRemoteState> _logger;
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;
    
    private int Count { get; set; }

    public ConnectionRemoteState(ISocket socket, IClientConnected clientConnected,
        ILogger<ConnectionRemoteState> logger)
    {
        _socket = socket;
        _clientConnected = clientConnected;
        _logger = logger;
        
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        _globalEventBus.Subscribe<ConnectionStates, CancellationToken>(
             async void (newState, cts) =>
            {
                try
                {
                    if (newState != ConnectionStates.Disconnected &&
                        newState != ConnectionStates.NoAuthenticated) return;

                    await MonitoringConnectionWorkAsync(newState, cts);
                }
                catch (Exception e)
                {
                    _logger.LogInformation("An error occurred when trying to receive a new state:" +
                                           " {state} of connection to the remote. Error: {Message}", newState, e);
                    _globalEventBus.Publish(ApplicationState.Restart, cts);
                }
            });
        
    }
    
    public async Task MonitoringConnectionWorkAsync(ConnectionStates state, CancellationToken cts = default)
    {
        await Task.Delay(TimeSpan.FromSeconds(20), cts);
        var socketWrapper = _clientConnected.GetSocketActive();
        
        if (socketWrapper is null)
        {
            _logger.LogCritical("No information from the client's " +
                                "socket was found in the reconnecting " +
                                "startup. Check the objects {nameObj}", nameof(socketWrapper));
            return;
        }
        
        await _socket.ReconnectAsync(socketWrapper, TypeAuthMode.RequireAuthentication, cts);
    }
    //
    // public async Task MonitoringConnectionWorkAsync(CancellationToken cts = default)
    // {
    //     _ = ReconnectAuthAsync(cts);
    //
    //     var clientInfo = _clientConnected.GetClientInfo();
    //
    //     try
    //     {
    //         while (!cts.IsCancellationRequested)
    //         {
    //             if (clientInfo is null)
    //             {
    //                 clientInfo = _clientConnected.GetClientInfo();
    //                 await Task.Delay(TimeSpan.FromMinutes(1), cts);
    //                 continue;
    //             }
    //
    //             if (!CommunicationStateReceiveAndSend.IsConnected)
    //             {
    //                 logger.LogCritical("Remote connection has been lost, entering the reconnection mode!.");
    //
    //                 await ReconnectAsync(clientInfo.SocketWrapper!, cts);
    //             }
    //
    //             await Task.Delay(TimeSpan.FromMinutes(1), cts);
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         logger.LogCritical(ex, "An error occurred when tryin" +
    //                                "g to reconnect with the remote!. ClientInfo: {ClientInfo}", clientInfo);
    //         throw new Exception();
    //     }
    // }
    //
    // private async Task ReconnectAuthAsync(CancellationToken cts)
    // {
    //     var objSocketSslStream = _clientConnected.GetObjSocketSslStream();
    //
    //     try
    //     {
    //         while (!cts.IsCancellationRequested)
    //         {
    //             try
    //             {
    //                 if (objSocketSslStream?.SocketWrapper?.InnerSocket is null)
    //                 {
    //                     objSocketSslStream = _clientConnected.GetObjSocketSslStream();
    //                     await Task.Delay(TimeSpan.FromMinutes(1), cts);
    //                     continue;
    //                 }
    //
    //                 if (!CommunicationStateReceiveAndSend.IsAuthenticated) continue;
    //
    //                 logger.LogCritical(
    //                     "Remote connection has been lost authenticate, entering the reconnection mode!.");
    //
    //                 await ReconnectAsync(objSocketSslStream.SocketWrapper, cts);
    //                 await Task.Delay(TimeSpan.FromMinutes(1), cts);
    //             }
    //             catch (OperationCanceledException e)
    //             {
    //                 logger.LogInformation("Operation to authenticate" +
    //                                       " with Remote to listen to a timeout. Message: {Message}", e.Message);
    //                 await ReconnectAuthAsync(cts);
    //             }
    //         }
    //     }
    //     catch (OperationCanceledException e)
    //     {
    //         logger.LogInformation("Operation to authenticate" +
    //                               " with Remote to listen to a timeout. Message: {Message}", e.Message);
    //         throw new OperationCanceledException();
    //     }
    //     catch (Exception e)
    //     {
    //         logger.LogCritical("An error occurred when " +
    //                            "trying to reconnect with the remote. Error: {Message}", e.Message);
    //         throw new Exception();
    //     }
    // }
    //
    // private async Task ReconnectAsync(ISocketWrapper socketWrapper, CancellationToken cts)
    // {
    //     await _socketMiring.ReconnectAsync(socketWrapper,
    //         TypeAuthMode.RequireAuthentication, cts);
    // }
}