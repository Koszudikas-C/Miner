using LibCommunicationStatus.Entities;
using LibDto.Dto;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Entities.Enum;

namespace ApiRemoteWorkClientBlockChain.Interface;

public interface IManagerConnection
{
    Task<ApiResponse<object>> InitializeAsync(ConnectionConfig connectionConfig, TypeAuthMode typeAuthMode,
        CancellationToken cts = default);

    Task<ApiResponse<object>> SendFileConfigVariableAsync(
        ConfigCryptographDto configCryptographDto,
        Guid clientId, CancellationToken cts = default);
}