using System.Net.Sockets;
using LibCommunicationStatus;
using LibHandler.EventBus;
using LibRemoteAndClient.Entities.Client;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSend.Interface;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Entities.Enum;
using LibSocketAndSslStream.Interface;
using LibRemoteAndClient.Entities.Client;

namespace LibSocket.Service;

public class SocketClientService(IListenerClient listenerClient,
    ISend<GuidTokenAuth> send) : ISocketMiring
{
    private readonly IListenerClient _listenerClient = listenerClient;
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;

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
        _listenerClient.ConnectedAct += async (socket) =>
            await OnSocketConnectedClientAuth(socket, TypeAuthMode.RequireAuthentication, 
                cts);

        await _listenerClient.ReconnectAsync(typeAuthMode, port, maxConnection, cts);
    }

    private async Task StartClientAsync(uint port,
        int maxConnection, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        _listenerClient.ConnectedAct += async (socket) =>
            await OnSocketConnectedClientAuth(socket, typeAuthMode, cts);

        await _listenerClient.StartAsync(typeAuthMode, port, 0, cts);

        CommunicationStatus.SetSending(true);
    }

    private async Task OnSocketConnectedClientAuth(Socket socket, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        var clientInfo = await SendGuidTokenAsync(socket, cts);
        
        MapperTypeObj(clientInfo, typeAuthMode);
        
        CommunicationStatus.SetConnected(true);
    }

    private async Task<ClientInfo> SendGuidTokenAsync(Socket socket, CancellationToken cts = default)
    {
        try
        {
            var token = new GuidTokenAuth();
            var clientInfo = new ClientInfo
            {
                Id = token.GuidTokenGlobal,
                SocketWrapper = new SocketWrapper(socket)
            };
            
            await send.SendAsync(token, clientInfo, TypeSocketSsl.Socket, cts);

            return clientInfo;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void MapperTypeObj(ClientInfo clientInfo,
        TypeAuthMode typeAuthMode)
    {
        if (typeAuthMode == TypeAuthMode.AllowAnonymous)
        {
            PublishTyped(clientInfo);
        }
        else
        {
            var sslStreamObj = new ObjSocketSslStream
            {
                Id = clientInfo.Id,
                SocketWrapper = new SocketWrapper(clientInfo.SocketWrapper!.InnerSocket)
            };
            PublishTyped(sslStreamObj);
        }
    }

    private void PublishTyped<T>(T data)
    {
        _globalEventBusClient.Publish(data);
    }
}