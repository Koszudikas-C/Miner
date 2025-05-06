using LibRemoteAndClient.Entities.Remote.Client;

namespace ApiRemoteWorkClientBlockChain.Entities.Interface;

public interface IClientConnected
{
    List<ClientInfo> AddClientInfo(ClientInfo clientInfo);
    List<ClientInfo> RemoveClientInfo(ClientInfo clientInfo);
    List<ClientInfo> UpdateClientInfo(ClientInfo clientInfo);
    List<ClientMine> AddClientMine(ClientMine clientMine);
    List<ClientMine> UpdateClientMine(ClientMine clientMine);
    
    ClientInfo GetClientInfoLastRequirement();
    ClientInfo GetClientInfo(Guid clientId, bool removeLastRequirement = false);
    List<ClientInfo> GetClientInfos();
}