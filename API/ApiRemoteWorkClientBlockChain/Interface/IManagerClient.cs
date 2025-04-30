using LibCommunicationStatus.Entities;
using LibCryptography.Entities;
using LibDto.Dto;
using LibRemoteAndClient.Entities.Remote.Client;
using LibSocketAndSslStream.Entities;

namespace ApiRemoteWorkClientBlockChain.Interface;

public interface IManagerClient
{
    ApiResponse<ClientInfo> GetAllClientInfo(int page, int pageSize);
    ApiResponse<ClientInfo> GetClientInfo(Guid clientId);

    Task<ApiResponse<object>> SendFileConfigVariableAsync(ConfigCryptographDto configCryptographDto,
        Guid clientId, CancellationToken cts = default);
}