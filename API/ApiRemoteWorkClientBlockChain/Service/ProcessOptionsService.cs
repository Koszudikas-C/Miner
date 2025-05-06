using ApiRemoteWorkClientBlockChain.Entities.Interface;
using LibClassManagerOptions.Entities.Enum;
using LibClassProcessOperations.Interface;
using LibHandler.EventBus;
using LibReceive.Interface;
using LibRemoteAndClient.Enum;
using LibRemoteAndClient.Interface;

namespace ApiRemoteWorkClientBlockChain.Service;

public class ProcessOptionsService : IProcessOptions
{
    private readonly IOperationFactory _operationFactory;
    private readonly IClientConnected _clientConnected;
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;

    public ProcessOptionsService(IOperationFactory operationFactory, IClientConnected clientConnected)
    {
        _operationFactory = operationFactory;
        _clientConnected = clientConnected;
    }

    public async Task IsProcessAuthSocks5Async(CancellationToken cts = default)
    {
        var clientInfo = _clientConnected.GetClientInfoLastRequirement();

        if (clientInfo == null! || !clientInfo.SocketWrapper!.Connected)
            _globalEventBusRemote.Publish(TypeManagerResponseOperations.NotFound);

        var operation = await _operationFactory.CreateAuthSocks5Operation();

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