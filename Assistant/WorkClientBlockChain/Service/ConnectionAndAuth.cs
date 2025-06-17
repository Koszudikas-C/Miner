using LibCommunicationStateClient.Entities.Enum;
using LibException;
using LibHandlerClient.Entities;
using LibSocketAndSslStreamClient.Entities.Enum;
using LibSocketAndSslStreamClient.Interface;
using WorkClientBlockChain.Interface;

namespace WorkClientBlockChain.Service;

public class ConnectionAndAuth(ISocket socket,
    ILogger<ConnectionAndAuth> logger) : IConnectionAndAuth
{
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;
     
    public async Task ConnectAndAuthAsync(CancellationToken cts = default)
    {
        try
        { 
            socket.InitializeAsync(5051, TypeAuthMode.RequireAuthentication,
                cts).ConfigureAwait(false).GetAwaiter();
            await Task.CompletedTask;
        }
        catch (Exception e)
        {
            logger.LogCritical("An error occurred when trying to connect or" +
             "attentive with the server. Error: {Message}", e);
        }
    }
}
