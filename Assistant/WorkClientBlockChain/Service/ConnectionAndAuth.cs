using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Entities.Enum;
using LibSocketAndSslStream.Interface;
using WorkClientBlockChain.Interface;

namespace WorkClientBlockChain.Service;

public class ConnectionAndAuth(ISocketMiring socketMiring, ILogger<ConnectionAndAuth> logger) : IConnectionAndAuth
{
    public async Task ConnectAndAuthAsync(ConnectionConfig cooConnectionConfig, CancellationToken cts = default)
    {
        try
        {
            await socketMiring.InitializeAsync(cooConnectionConfig.Port, cooConnectionConfig.MaxConnections,
                TypeRemoteClient.Client, TypeAuthMode.RequireAuthentication, cts);
        }
        catch (Exception e)
        {
            logger.LogError($"Error starting SocketMiringService in mode Client. Error: {e.Message}");
        }
    }
}