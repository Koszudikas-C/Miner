using LibCommunicationStateClient.Entities;
using LibSocketAndSslStreamClient.Entities.Enum;
using LibSocketAndSslStreamClient.Interface;
using WorkClientBlockChain.Interface;

namespace WorkClientBlockChain.Service;

public class ConnectionAndAuth(ISocketMiring socketMiring,
    ILogger<ConnectionAndAuth> logger) : IConnectionAndAuth
{
    public async Task ConnectAndAuthAsync(CancellationToken cts = default)
    {
        try
        {
            await socketMiring.InitializeAsync(5051, TypeAuthMode.RequireAuthentication,
              cts);
        }
        catch (Exception e)
        {
            logger.LogCritical("An error occurred when trying to connect or" +
             "attentive with the server. Error: {Message}", e.Message);
            
        }
    }
}
