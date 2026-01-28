using System.Net.Sockets;
using LibCommunicationStatus;
using LibHandler.EventBus;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Entities.Enum;
using LibSocketAndSslStream.Interface;

namespace LibSocket.Service;

public class SocketClientService(
    IListenerClient listenerClient) : ISocketMiring
{
    private readonly IListenerClient _listenerClient = listenerClient;
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance;

    public async Task InitializeAsync(uint port, int maxConnection,
        TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        if (port is < 1000 or > 9999)
            throw new Exception("Port number must be a 4-digit number between 1000 and 9999.");

        try
        {
            await StartClientAsync(port, maxConnection, typeAuthMode, cts);
        }
        catch (Exception e)
        {
            throw new Exception($"It was not possible to start the connection to the server: {e.Message}");
        }
    }

    public async Task ReconnectAsync(uint port, int maxConnection,
        TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        try
        {
            await ReconnectionClientAsync(port, maxConnection, typeAuthMode, cts);
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to reconnect to the server: {e.Message}");
        }
    }

    private async Task ReconnectionClientAsync(uint port, int maxConnection,
        TypeAuthMode typeAuthMode, CancellationToken cts = default)
    {
        _listenerClient.ConnectedAct += OnSocketConnectedClientAuth;

        await _listenerClient.ReconnectAsync(typeAuthMode, port, maxConnection, cts);
    }

    private async Task StartClientAsync(uint port,
        int maxConnection, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        _listenerClient.ConnectedAct +=  OnSocketConnectedClientAuth;

        await _listenerClient.StartAsync(typeAuthMode, port, 0, cts);

        CommunicationStatus.SetSending(true);
    }

    private void OnSocketConnectedClientAuth(Socket socket)
    {
        MapperTypeObj(socket);

        CommunicationStatus.SetConnected(true);
    }


    private void MapperTypeObj(Socket socket)
    {
        var sslStreamObj = new ObjSocketSslStream
        {
            SocketWrapper = new SocketWrapper(socket)
        };
        PublishTyped(sslStreamObj);
    }

    private void PublishTyped<T>(T data)
    {
        _globalEventBusClient.Publish(data);
    }
}