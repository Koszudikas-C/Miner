using System.Net.Security;
using WorkClientBlockChain.Interface;

namespace WorkClientBlockChain.Service;

public class AppClientService : IAppClient
{
    private SslStream _sslStream;
    public Task InstallAppAsync(CancellationToken cts)
    {
        throw new NotImplementedException();    
    }

    public Task UpdateAppAsync(CancellationToken cts)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAppAsync(CancellationToken cts)
    {
        throw new NotImplementedException();
    }

    public Task IsProcessRunningAsync(CancellationToken cts)
    {
        throw new NotImplementedException();
    }
}
