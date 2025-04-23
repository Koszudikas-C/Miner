namespace WorkClientBlockChain.Interface;

public interface IProcessOptions
{
    Task<bool> IsProcessAuthSocks5(CancellationToken cts = default);

    Task IsProcessCheckAppClientBlockChain(CancellationToken cts = default);
    
    Task IsProcessDownloadAppClientBlockChain(CancellationToken cts = default);
    
    Task IsProcessLogs(CancellationToken cts = default);
    
    Task IsProcessStatusConnection(CancellationToken cts = default);
    
    Task IsProcessStatusTransaction(CancellationToken cts = default);
}