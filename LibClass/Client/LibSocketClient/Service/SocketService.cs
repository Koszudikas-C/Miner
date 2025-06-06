using System.Net.Sockets;
using LibCommunicationStateClient.Entities;
using LibEntitiesClient.Entities;
using LibHandlerClient.Entities;
using LibSocketAndSslStreamClient.Entities.Enum;
using LibSocketAndSslStreamClient.Interface;

namespace LibSocketClient.Service;

public class SocketService(
    IListener listener) : ISocketMiring
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

    public async Task ReconnectAsync(uint port, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        try
        {
            Console.WriteLine("Reconectando ao servidor");
            await ReconnectionClientAsync(port, typeAuthMode, cts);
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to reconnect to the server: {e.Message}");
        }
    }

    private async Task ReconnectionClientAsync(uint port,
        TypeAuthMode typeAuthMode, CancellationToken cts = default)
    {
        _listener.ConnectedAct += async (handle, ctsa) =>
         await OnSocketConnectedClientAuth(handle, ctsa);

        await _listener.ReconnectAsync(typeAuthMode, port, cts);
    }

    private async Task StartClientAsync(uint port, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {

        _listener.ConnectedAct += async (handle, ctsa) =>
        {
            await OnSocketConnectedClientAuth(handle, ctsa);
        };


        await _listener.StartAsync(typeAuthMode, port, cts);

        CommunicationStateReceiveAndSend.SetSending(true);
    }

    private async Task OnSocketConnectedClientAuth(Socket socket, CancellationToken cts)
    {
        await MapperTypeObj(socket, cts);

        CommunicationStateReceiveAndSend.SetConnected(true);
    }

    private async Task MapperTypeObj(Socket socket, CancellationToken cts)
    {
        var sslStreamObj = new ObjSocketSslStream
        {
            SocketWrapper = new SocketWrapper(socket)
        };
        
        Console.WriteLine(sslStreamObj.SocketWrapper.PortRemote);
        await PublishTyped(sslStreamObj, cts);
    }

    private async Task PublishTyped<T>(T data, CancellationToken cts)
    {
        _globalEventBusClient.Publish(data);
        await _globalEventBusClient.PublishAsync(data, cts);
    }
}
