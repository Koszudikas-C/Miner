using System.Net.Sockets;
using LibCommunicationStateRemote.Entities;
using LibCommunicationStatusRemote.Entities.Enum;
using LibEntitiesRemote.Entities;
using LibHandlerRemote.Entities;
using LibSocketAndSslStreamRemote.Entities;
using LibSocketAndSslStreamRemote.Entities.Enum;
using LibSocketAndSslStreamRemote.Interface;

namespace LibSocketRemote.Service;

public sealed class SocketService(
    IListener listenerRemote) : ISocket
{
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;

    public void InitializeRemote(uint port, int maxConnection, TypeAuthMode typeAuthMode)
    {
        if (port is < 1000 or > 9999)
            throw new ArgumentException("Port number must be a 4-digit number between 1000 and 9999.");

        StartRemoteAsync(port, maxConnection, typeAuthMode);
    }

    private void StartRemoteAsync(uint port, int maxConnection,
        TypeAuthMode typeAuthMode)
    {
        listenerRemote.ConnectedActAsync += async (socket, cts) =>
            await OnSocketConnectRemoteAuthAsync(socket, typeAuthMode, cts);

        _ = Task.Run(() => { listenerRemote.StartAsync(typeAuthMode, port, maxConnection); });
    }

    private async Task OnSocketConnectRemoteAuthAsync(Socket socket, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        await MapperTypeObj(socket, typeAuthMode, cts);
    }

    private async Task MapperTypeObj(Socket socket, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        if (typeAuthMode == TypeAuthMode.AllowAnonymous)
        {
            var clientInfo = new ClientInfoAuth(socket);
            ClientAuthState.AddClientToAuthState(clientInfo);
            await PublishTyped(clientInfo, cts);
        }
        else
        {
            var socketConnected = new SocketsConnectedEvent(socket);
            await _globalEventBus.PublishAsync(socketConnected, cts);
            
            var objSocket = new ObjSocketSslStream(socket);
            ClientAuthState.AddClientToAuthState(objSocket);
            await PublishTyped(objSocket, cts);
        }
    }

    private async Task PublishTyped<T>(T data, CancellationToken cts = default)
    {
        await _globalEventBus.PublishAsync(data, cts);
    }
}