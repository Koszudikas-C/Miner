using LibRemoteAndClient.Entities.Remote.Client;
using LibSocket.Entities;

namespace ApiRemoteWorkClientBlockChain.Interface;

public interface IManagerConnection
{
    Task InitializeAsync(ConnectionConfig connectionConfig,
        CancellationToken cts = default);

    IReadOnlyCollection<ClientInfo> GetClientLast();
}