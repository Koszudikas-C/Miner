using LibClassManagerOptions.Entities.Enum;
using LibSocketAndSslStream.Interface;
using LibClassProcessOperations.Interface;
using LibHandler.EventBus;
using WorkClientBlockChain.Utils.Interface;
using WorkClientBlockChain.Connection.Interface;

namespace WorkClientBlockChain.Service;

public class ProcessOptions(ILogger<ProcessOptions> logger, IClientContext clientContext,
    ISocketMiring socketMiring) 
    : IProcessOptions
{
    private readonly ILogger<ProcessOptions> _logger = logger; 
    private readonly IClientContext _clientContext = clientContext;
    private readonly ISocketMiring _socketMiring = socketMiring;

    private readonly GlobalEventBusClient _globalEventBusRemote =
        GlobalEventBusClient.Instance!; 

    public async Task IsProcessAuthSocks5Async(CancellationToken cts = default)
    {
        var clientInfo = _clientContext.GetClientInfo();
        while (true)
        {
            if (_clientContext.GetClientInfo()!.SslStreamWrapper == null)
            {
                await Task.Delay(1000, cts);
                continue;
            }

            if (!_clientContext.GetClientInfo()!.SslStreamWrapper!.IsAuthenticated)
                continue;
            
            _logger.LogInformation($"Client {clientInfo!.Id} Authenticated()");
            _globalEventBusRemote.Publish(TypeManagerResponseOperations.Success); 
            break;
        }
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