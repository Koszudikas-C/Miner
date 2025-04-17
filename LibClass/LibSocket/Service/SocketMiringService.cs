using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using LibCommunicationStatus;
using LibHandler.EventBus;
using LibJson.Util;
using LibReceive;
using LibReceive.Entities;
using LibRemoteAndClient.Entities.Client;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSend.Entities;
using LibSend.Interface;
using LibSend.Service;
using LibSocket.Connection;
using LibSocket.Entities.Enum;
using LibSocket.Interface;
using LibSsl.Entities;

namespace LibSocket.Service;

public class SocketMiringService : ISocketMiring
{
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;

    public async Task InitializeAsync(uint port, int maxConnection,
        TypeRemoteClient typeEnum, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        try
        {
            if (port is < 1000 or > 9999)
                throw new Exception("Port number must be a 4-digit number between 1000 and 9999.");

            
            switch (typeEnum)
            {
                case TypeRemoteClient.Client:
                    await StartClientAsync(port, maxConnection, typeAuthMode, typeEnum, cts);
                    break;
                case TypeRemoteClient.Remote:
                    StartRemote(port, maxConnection, typeAuthMode, typeEnum);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeEnum), typeEnum, null);
            }
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"Error starting SocketMiringService in mode {typeEnum}: {e}");
            throw new InvalidOperationException("Failed to boot the socket service.", e);
        }
    }

    private async Task StartClientAsync(uint port,
        int maxConnection, TypeAuthMode typeAuthMode,
        TypeRemoteClient typeRemoteClient, CancellationToken cts = default)
    {
        var listener = new ListenerClient(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        
        listener.ConnectedAct += async (socket) =>
            await OnSocketConnectedClientAuth(socket, typeAuthMode,
                typeRemoteClient, cts);

        await listener.StartAsync(typeAuthMode, port, 0, cts);

        CommunicationStatus.SetSending(true);
    }

    private void StartRemote(uint port, int maxConnection,
        TypeAuthMode typeAuthMode, TypeRemoteClient typeRemoteClient)
    {
        var listener = new ListenerRemote(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        listener.ConnectedAct += async (socket) =>
            await OnSocketConnectRemoteAuthAsync(socket, typeAuthMode, typeRemoteClient);
        
        Task.Run(() => listener.StartAsync(typeAuthMode, port, maxConnection));
    }

    private async Task OnSocketConnectedClientAuth(Socket socket, TypeAuthMode typeAuthMode
        ,TypeRemoteClient typeRemoteClient, CancellationToken cts = default)
    {
        var token = await SendGuidTokenAsync(socket, cts);
        MapperTypeObj(token, socket, typeAuthMode, typeRemoteClient);
    }

    private async Task OnSocketConnectRemoteAuthAsync(Socket socket, TypeAuthMode typeAuthMode,
        TypeRemoteClient typeRemoteClient, CancellationToken cts = default)
    {
        await ReceiveGuidTokenAsync(socket, typeAuthMode, typeRemoteClient, cts);
    }

    private async Task ReceiveGuidTokenAsync(Socket socket, TypeAuthMode typeAuthMode
        ,TypeRemoteClient typeRemoteClient, CancellationToken cts = default)
    {
        var receive = new Receive(socket);

        receive.OnReceivedAct += (json) =>
        {
            if (JsonElementConvertRemote.ConvertToObject(json) is not GuidTokenAuth token) return;

            MapperTypeObj(token.GuidTokenGlobal, socket, typeAuthMode, typeRemoteClient);
        };

        await receive.ReceiveDataAsync(cts);
    }

    private static async Task<Guid> SendGuidTokenAsync(Socket socket, CancellationToken cts = default)
    {
        var send = new Send<GuidTokenAuth>(socket);
        var token = new GuidTokenAuth();

        await send.SendAsync(token, cts);

        return token.GuidTokenGlobal;
    }

    private void MapperTypeObj(Guid token, Socket socket,
        TypeAuthMode typeAuthMode, TypeRemoteClient typeRemoteClient)
    {
        if (typeAuthMode == TypeAuthMode.AllowAnonymous)
        {
            var clientInfo = new ClientInfo { Id = token, Socket = socket };
            PublishTyped(clientInfo, typeRemoteClient);
        }
        else
        {
            var sslStreamObj = new ObjSocketSslStream { Id = token, Socket = socket };
            PublishTyped(sslStreamObj, typeRemoteClient);
        }
    }
    
    private void PublishTyped<T>(T data, TypeRemoteClient remoteClient)
    {
        if (remoteClient == TypeRemoteClient.Client)
            _globalEventBusClient.Publish(data);
        else
            _globalEventBusRemote.Publish(data);
    }
}