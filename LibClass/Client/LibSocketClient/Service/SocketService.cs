using System.Data;
using System.Net.Sockets;
using LibCommunicationStateClient.Entities;
using LibCommunicationStateClient.Entities.Enum;
using LibEntitiesClient.Entities;
using LibEntitiesClient.Interface;
using LibHandlerClient.Entities;
using LibSocketAndSslStreamClient.Entities.Enum;
using LibSocketAndSslStreamClient.Interface;

namespace LibSocketClient.Service;

public class SocketService(
    IListener listener) : ISocket
{
    private readonly IListener _listener = listener;
    private readonly GlobalEventBus _globalEventBusClient = GlobalEventBus.Instance;

    public async Task InitializeAsync(uint port, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        if (port is < 1000 or > 9999)
            throw new Exception("Port number must be a 4-digit number between 1000 and 9999.");

        try
        {
            await StartClientAsync(port, typeAuthMode, cts);
        }
        catch (Exception e)
        {
            throw new Exception($"It was not possible to start the connection to the server: {e.Message}");
        }
    }

    public async Task ReconnectAsync(ISocketWrapper socketWrapper,
        TypeAuthMode typeAuthMode, CancellationToken cts = default)
    {
        _listener.ConnectedAct += async (handle, ctsa) =>
             await OnSocketConnectedClientAuth(handle, ctsa);

        await _listener.ReconnectAsync(socketWrapper.InnerSocket, typeAuthMode, cts);
    }

    private async Task StartClientAsync(uint port, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        _listener.ConnectedAct += async (handle, ctsa) =>
             await OnSocketConnectedClientAuth(handle, ctsa);
        
        await _listener.StartAsync(typeAuthMode, port, cts);
    }

    private async Task OnSocketConnectedClientAuth(Socket socket, CancellationToken cts)
    {
        await MapperTypeObj(socket, cts);
    }

    private async Task MapperTypeObj(Socket socket, CancellationToken cts)
    {
        var sslStreamObj = new ObjSocketSslStream
        {
            SocketWrapper = new SocketWrapper(socket)
        };
        
        await PublishTypedAsync(sslStreamObj, cts);
    }

    private async Task PublishTypedAsync<T>(T data, CancellationToken cts)
    {
        _globalEventBusClient.Publish(ConnectionStates.Connecting, cts);
        _globalEventBusClient.Publish(data);
       await _globalEventBusClient.PublishAsync(data, cts);

        await Task.CompletedTask;
    }
}