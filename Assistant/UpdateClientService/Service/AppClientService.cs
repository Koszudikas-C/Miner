using System.Net.Security;
using UpdateClientService.Interface;

namespace UpdateClientService.Service;

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
