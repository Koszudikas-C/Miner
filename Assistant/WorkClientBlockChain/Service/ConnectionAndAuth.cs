using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Entities.Enum;
using LibSocketAndSslStream.Interface;
using WorkClientBlockChain.Interface;

namespace WorkClientBlockChain.Service;

public class ConnectionAndAuth(ISocketMiring socketMiring, 
    ILogger<ConnectionAndAuth> logger, IAuthSsl authSsl) : IConnectionAndAuth
{
    public async Task ConnectAndAuthAsync(CancellationToken cts = default)
    {
        try
        {
            await socketMiring.InitializeAsync(
                5051, 0,
                TypeAuthMode.RequireAuthentication, cts);
        }
        catch (Exception e)
        {
            logger.LogError($"Error starting SocketMiringService in mode Client. Error: {e.Message}");
        }
    }
}