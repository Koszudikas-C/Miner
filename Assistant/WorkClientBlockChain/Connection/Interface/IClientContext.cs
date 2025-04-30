using LibRemoteAndClient.Entities.Remote.Client;

namespace WorkClientBlockChain.Connection.Interface;

public interface IClientContext
{
    ClientInfo? GetClientInfo();
    void SetClientInfo(ClientInfo? info);
    
    void Reset();
}
