using LibHandler.EventBus;
using LibRemoteAndClient.Entities.Client;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSend.Interface;
using LibSocket.Entities.Enum;
using LibSsl.Entities;
using UpdateClientService.Interface;

namespace UpdateClientService.Service;

public class ConnectionAndAuth(
    LibSocket.Interface.ISocketMiring
        socketMiring,
    ISend<GuidTokenAuth> send) : IConnectionAndAuth
{
    public async Task ConnectAndAuthAsync(CancellationToken cts = default)
    {
        await socketMiring.InitializeAsync(9090, 0,
            TypeRemoteClient.Client, TypeAuthMode.AllowAnonymous, cts);
    }
    
}