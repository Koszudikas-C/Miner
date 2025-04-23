using System.Net.Sockets;
using ApiRemoteWorkClientBlockChain.Entities;
using LibClassManagerOptions.Entities.Enum;
using LibClassManagerOptions.Interface;
using LibHandler.EventBus;
using LibSend.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;

namespace ApiRemoteWorkClientBlockChain.Service;

public class ManagerOptionsService : IManagerOptions
{
    private readonly ISend<TypeManagerOptions> _send;
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;
    private readonly ClientConnected _clientConnected = ClientConnected.Instance!;
    
    public ManagerOptionsService(ISend<TypeManagerOptions> send)
    {
        _send = send;
        
        _globalEventBusRemote.Subscribe<TypeManagerResponseOperations>(async void
        (handler) => await ResponseOptions(handler));
    }
    
    public async Task InitializeOptions(TypeManagerOptions typeManagerOptions, 
        CancellationToken cts = default)
    {
        var clientInfo = GetClientInfoManager();
        var typeSocketSsl = GetSocketSsl(clientInfo);
        switch (typeManagerOptions)
        {
            case TypeManagerOptions.AuthSocks5:
                await _send.SendAsync(TypeManagerOptions.AuthSocks5, clientInfo!, typeSocketSsl, cts);
                break;
            case TypeManagerOptions.CheckAppClientBlockChain:
                await _send.SendAsync(TypeManagerOptions.CheckAppClientBlockChain,
                    clientInfo!, typeSocketSsl, cts);
                break;
            case TypeManagerOptions.DownloadAppClientBlockChain:
                await _send.SendAsync(TypeManagerOptions.DownloadAppClientBlockChain,
                    clientInfo!, typeSocketSsl, cts);
                break;
            case TypeManagerOptions.Logs:
                await _send.SendAsync(TypeManagerOptions.Logs, clientInfo!, typeSocketSsl, cts);
                break;
            case TypeManagerOptions.StatusConnection:
                await _send.SendAsync(TypeManagerOptions.StatusConnection, clientInfo!,
                    typeSocketSsl, cts);
                break;
            case TypeManagerOptions.StatusTransaction:
                await _send.SendAsync(TypeManagerOptions.StatusTransaction, clientInfo!,
                    typeSocketSsl, cts);
                break;
            case TypeManagerOptions.CancelOperations:
                await _send.SendAsync(TypeManagerOptions.CancelOperations, clientInfo!,
                    typeSocketSsl, cts);
                break;
            case TypeManagerOptions.Error:
            default:
                await _send.SendAsync(TypeManagerOptions.Error, clientInfo!, typeSocketSsl, cts);
                break;
        }
    }

    public Task ResponseOptions(TypeManagerResponseOperations typeManagerResponseOperations,
        CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }
    
    private ClientInfo? GetClientInfoManager()
    {
        var clientInfo = _clientConnected.GetClientInfoLastRequirement();

        if (clientInfo != null! && clientInfo.SocketWrapper!.Connected) return clientInfo;
        
        clientInfo!.SocketWrapper!.InnerSocket.Close();
        throw new SocketException();
    }

    private static TypeSocketSsl GetSocketSsl(ClientInfo? clientInfo)
    {
        if(clientInfo is not null && clientInfo.SslStreamWrapper!.IsAuthenticated)
            return TypeSocketSsl.SslStream;
        
        return TypeSocketSsl.Socket;
    }
}