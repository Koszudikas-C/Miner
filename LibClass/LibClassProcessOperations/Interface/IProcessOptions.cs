using LibClassManagerOptions.Entities.Enum;

namespace LibClassProcessOperations.Interface;

public interface IProcessOptions
{
    Task IsProcessAuthSocks5Async(CancellationToken cts = default);

    Task IsProcessCheckAppClientBlockChain(CancellationToken cts = default);
    
    Task IsProcessDownloadAppClientBlockChain(CancellationToken cts = default);
    
    Task IsProcessLogs(CancellationToken cts = default);
    
    Task IsProcessStatusConnection(CancellationToken cts = default);
    
    Task IsProcessStatusTransaction(CancellationToken cts = default);
}