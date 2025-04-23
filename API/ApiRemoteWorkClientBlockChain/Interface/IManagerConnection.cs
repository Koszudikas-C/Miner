using LibCommunicationStatus.Entities;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Entities.Enum;

namespace ApiRemoteWorkClientBlockChain.Interface;

public interface IManagerConnection
{
    Task<ApiResponse<object>> InitializeAsync(ConnectionConfig connectionConfig, TypeAuthMode typeAuthMode,
        CancellationToken cts = default);
}