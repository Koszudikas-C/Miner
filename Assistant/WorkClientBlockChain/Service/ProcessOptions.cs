using LibRemoteAndClient.Enum;
using LibSsl.Enum;
using LibSsl.Interface;
using WorkClientBlockChain.Connection;
using WorkClientBlockChain.Interface;

namespace WorkClientBlockChain.Service;

public class ProcessOptions(ILogger<ProcessOptions> logger, IAuthSsl authSsl) : IProcessOptions
{
    public readonly ILogger<ProcessOptions> _logger = logger; 
    private readonly IAuthSsl _authSsl = authSsl;
    
    public async Task<bool> IsProcessAuthSocks5(CancellationToken cts = default)
    {
        var clientInfo = ClientContext.GetClientInfo();
        
        if (clientInfo == null || !clientInfo.Socket!.Connected) return false;
        
        await _authSsl.AuthenticateAsync(clientInfo.Socket, 
            TypeRemoteClient.Client, clientInfo.Id, cts)!.ConfigureAwait(false);

        while (true)
        {
            if (ClientContext.GetClientInfo()!.SslStream == null)
            {
                await Task.Delay(1000, cts);
                continue;
            }
            
            if (ClientContext.GetClientInfo()!.SslStream!.IsAuthenticated)
            {
                _logger.LogInformation($"Client {clientInfo.Id} Authenticated()");
                return true;
            }
            
            await Task.Delay(1000, cts);
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