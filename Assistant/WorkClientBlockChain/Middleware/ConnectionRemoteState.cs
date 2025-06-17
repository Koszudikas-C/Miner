using LibCommunicationStateClient.Entities.Enum;
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
        _globalEventBus.SubscribeAsync<ConnectionStates>( async (newState, cts) =>
        {
            if (newState != ConnectionStates.Disconnected &&
                newState != ConnectionStates.NoAuthenticated) return;

            await MonitoringConnectionWorkAsync(newState, cts);
        });
    }

    public async Task MonitoringConnectionWorkAsync(ConnectionStates state, CancellationToken cts = default)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(10), cts);
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
        catch (Exception e)
        {
            _logger.LogInformation("An error occurred when trying to receive a new state:" +
                                   " {state} of connection to the remote. Error: {Message}", state, e);
            _globalEventBus.Publish(ApplicationState.Restart, cts);
            throw new Exception();
        }
    }
}