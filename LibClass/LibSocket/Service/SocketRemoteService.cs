using System.Net.Sockets;
using LibHandler.EventBus;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Entities.Enum;
using LibSocketAndSslStream.Interface;
using GuidTokenAuth = LibRemoteAndClient.Entities.Remote.Client.GuidTokenAuth;

namespace LibSocket.Service;

public class SocketRemoteService(
    IListenerRemote listenerRemote,
    IReceive receive) : ISocketMiring
{
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance;

    public async Task InitializeAsync(uint port, int maxConnection, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        if (port is < 1000 or > 9999)
            throw new Exception("Port number must be a 4-digit number between 1000 and 9999.");

        try
        {
            StartRemote(port, maxConnection, typeAuthMode);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error when booting the remote server: {ex.Message}");
        }
    }

    public Task ReconnectAsync(uint port, int maxConnection,
        TypeAuthMode typeAuthMode, CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }

    private void StartRemote(uint port, int maxConnection,
        TypeAuthMode typeAuthMode)
    {
        listenerRemote.ConnectedAct += async (socket) =>
            await OnSocketConnectRemoteAuthAsync(socket, typeAuthMode);
        
        Task.Run(() => listenerRemote.StartAsync(typeAuthMode, port, maxConnection));
    }

    private async Task OnSocketConnectRemoteAuthAsync(Socket socket, TypeAuthMode typeAuthMode
        , CancellationToken cts = default)
    {
        MapperTypeObj(socket, typeAuthMode);
        await Task.CompletedTask;
        // await ReceiveGuidTokenAsync(socket, typeAuthMode, cts);
    }
    
    private async Task ReceiveGuidTokenAsync(Socket socket,
        TypeAuthMode typeAuthMode, CancellationToken cts = default)
    {
        try
        {
            var tcs = new TaskCompletionSource<Guid>();
            void Handler(GuidTokenAuth tokenAuth)
            {
                tcs.TrySetResult(tokenAuth.GuidTokenGlobal);
                // MapperTypeObj(tokenAuth.GuidTokenGlobal, socket, typeAuthMode);
                _globalEventBusRemote.Unsubscribe<GuidTokenAuth>(Handler);
            }
            
            _globalEventBusRemote.Subscribe<GuidTokenAuth>(Handler);
            var clientInfo = new ClientInfo { SocketWrapper = new SocketWrapper(socket) };
            
            await receive.ReceiveDataAsync(clientInfo, TypeSocketSsl.Socket, 0, cts);
            
            await tcs.Task.WaitAsync(TimeSpan.FromSeconds(5), cts);
        }
        catch (Exception e)
        {
            DisconnectSocket(socket);
            throw new Exception(e.Message);
        }
    }
    
    
    private void MapperTypeObj(Socket socket, TypeAuthMode typeAuthMode)
    {
        if (typeAuthMode == TypeAuthMode.AllowAnonymous)
        {
            var clientInfo = new ClientInfo { SocketWrapper = new SocketWrapper(socket) };
            PublishTyped(clientInfo);
        }
        else
        {
            var sslStreamObj = new ObjSocketSslStream
            {
                SocketWrapper = new SocketWrapper(socket)
            };
            
            PublishTyped(sslStreamObj);
        }
    }

    // private void MapperTypeObj(Guid token, Socket socket, TypeAuthMode typeAuthMode)
    // {
    //     if (typeAuthMode == TypeAuthMode.AllowAnonymous)
    //     {
    //         var clientInfo = new ClientInfo { Id = token, SocketWrapper = new SocketWrapper(socket) };
    //         PublishTyped(clientInfo);
    //     }
    //     else
    //     {
    //         var sslStreamObj = new ObjSocketSslStream
    //         {
    //             Id = token,
    //             SocketWrapper = new SocketWrapper(socket)
    //         };
    //         PublishTyped(sslStreamObj);
    //     }
    // }

    private static void DisconnectSocket(Socket socket) => socket.Close();

    private void PublishTyped<T>(T data)
    {
        _globalEventBusRemote.Publish(data);
    }
}