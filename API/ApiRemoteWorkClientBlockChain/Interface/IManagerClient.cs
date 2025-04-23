using LibCommunicationStatus.Entities;
using LibRemoteAndClient.Entities.Remote.Client;

namespace ApiRemoteWorkClientBlockChain.Interface;

public interface IManagerClient
{
    ApiResponse<ClientInfo> GetAllClientInfo(int page, int pageSize);
    ApiResponse<ClientInfo> GetClientInfo(Guid clientId);
}