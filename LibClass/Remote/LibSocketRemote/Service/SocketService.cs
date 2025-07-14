using System.Net.Sockets;
using LibCommunicationStateRemote.Entities;
using LibEntitiesRemote.Entities;
using LibEntitiesRemote.Interface;
using LibHandlerRemote.Entities;
using LibSocketAndSslStreamRemote.Entities.Enum;
using LibSocketAndSslStreamRemote.Interface;
using Microsoft.Extensions.Logging;

namespace LibSocketRemote.Service;

/// <summary>
/// Responsibility for calling the initialize of Remote.
/// Client socket publication.
/// </summary>
/// <param name="listenerRemote"></param>
/// <param name="logger"></param>
public sealed class SocketService(
    IListener listenerRemote, ILogger<SocketService> logger,
    IManagerSocketConnected managerSocketConnected) : ISocket
{
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;
    
    public void InitializeRemote(uint port, int maxConnection, TypeAuthMode typeAuthMode)
    {
        if (port is < 1000 or > 9999)
            throw new ArgumentException("Port number must be a 4-digit number between 1000 and 9999.");

        StartRemote(port, maxConnection, typeAuthMode);
    }

    private void StartRemote(uint port, int maxConnection,
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
            await PublishTypedAsync(clientInfo, clientInfo.SocketWrapper, cts);
        }
        else
        {
            var objSocket = new ObjSocketSslStream(socket);
            ClientConnected(objSocket.SocketWrapper);
            await PublishTypedAsync(objSocket, objSocket.SocketWrapper, cts);
        }
    }

    private void ClientConnected(ISocketWrapper socket)
    {
        managerSocketConnected.CheckStateSocket(socket);
    }

    private async Task PublishTypedAsync<T>(T data, ISocketWrapper socket, CancellationToken cts = default)
    {
        try
        {
            await _globalEventBus.PublishAsync(data, cts);
        }
        catch (OperationCanceledException e)
        {
            logger.LogWarning("Listen to a canceled operation probably" +
                               " timeout at the time of SSL/TLS authentication with remote. Client IP: {IP}. Error: {Message}",
                socket.RemoteEndPoint, e);
            ClientConnected(socket);
            throw;
        }
        catch (IOException e)
        {
            logger.LogCritical("There was an error " +
                                "when receiving possible client data possibly can " +
                                "be bots trying to seek information. bot IP: {IP}. Error: {Message}",
                socket.RemoteEndPoint, e);
            ClientConnected(socket);
            throw;
        }
        catch (Exception e)
        {
            logger.LogCritical("A generic error occurred check the past method " +
                                "for the client to check the reliability is integrity. IP: {IP}. Error: {Message}",
                socket.RemoteEndPoint, e);
            ClientConnected(socket);
            throw;
        }
        finally
        {
            socket.InnerSocket.Close();
        }
    }
}