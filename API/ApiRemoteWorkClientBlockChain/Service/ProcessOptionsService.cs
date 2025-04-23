using ApiRemoteWorkClientBlockChain.Entities.Interface;
using LibClassManagerOptions.Entities.Enum;
using LibClassProcessOperations.Interface;
using LibHandler.EventBus;
using LibReceive.Interface;
using LibRemoteAndClient.Enum;
using LibRemoteAndClient.Interface;

namespace ApiRemoteWorkClientBlockChain.Service;

public class ProcessOptionsService(LibSend.Interface.ISend<TypeManagerOptions> send,
   IReceive receive, IClientConnected clientConnected, IAwaitResult awaitResult) : IProcessOptions
{
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;
    private readonly IClientConnected _clientConnected = clientConnected;
    private readonly IAwaitResult _awaitResult = awaitResult;

    private TypeManagerResponseOperations? typeManagerResponseOperations;

    public async Task IsProcessAuthSocks5Async(CancellationToken cts = default)
    {
        var clientInfo = _clientConnected.GetClientInfoLastRequirement();

        if (clientInfo == null! || !clientInfo.SocketWrapper!.Connected)
            _globalEventBusRemote.Publish(TypeManagerResponseOperations.NotFound);

        await send.SendAsync(TypeManagerOptions.AuthSocks5, clientInfo!,
            TypeSocketSsl.Socket, cts).ConfigureAwait(false);

        await receive.ReceiveDataAsync(clientInfo!,
            TypeRemoteClient.Remote, 0, cts);
    }

    public Task IsProcessCheckAppClientBlockChain(CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }

    public Task IsProcessDownloadAppClientBlockChain(CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }

    public Task IsProcessLogs(CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }

    public Task IsProcessStatusConnection(CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }

    public Task IsProcessStatusTransaction(CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }
}