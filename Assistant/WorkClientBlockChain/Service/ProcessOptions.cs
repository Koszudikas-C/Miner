using WorkClientBlockChain.Interface;

namespace WorkClientBlockChain.Service;

public class ProcessOptions(ILogger<ProcessOptions> logger) : IProcessOptions
{
    public Task IsProcessAuthSocks5(CancellationToken cts = default)
    {
        throw new NotImplementedException();
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