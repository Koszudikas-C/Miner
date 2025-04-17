using LibCommunicationStatus.Entities;
using LibRemoteAndClient.Entities.Remote.Client;
using LibSocket.Entities;
using LibSocket.Entities.Enum;

namespace ApiRemoteWorkClientBlockChain.Interface;

public interface IManagerConnection
{
    Task<ApiResponse<object>> InitializeAsync(ConnectionConfig connectionConfig, TypeAuthMode typeAuthMode,
        CancellationToken cts = default);
}