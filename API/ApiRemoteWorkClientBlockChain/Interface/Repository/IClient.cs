using ApiRemoteWorkClientBlockChain.Entities;
using LibCommunicationStateRemote.Entities;
using LibEntitiesRemote.Entities.Client;

namespace ApiRemoteWorkClientBlockChain.Interface.Repository;

public interface IClient : IRepositoryBase<int, Client>
{
    Task<ApiResponse<bool>> AddListAsync(List<Client> clients, CancellationToken cts = default);
    Task<ApiResponse<bool>> UpdateListAsync(List<Client> clients, CancellationToken cts = default);
    Task<ApiResponse<Client>> GetClientByIp(string ip, CancellationToken cts = default);
}