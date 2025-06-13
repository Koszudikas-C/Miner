using LibEntitiesClient.Entities;
using LibEntitiesClient.Interface;

namespace WorkClientBlockChain.Connection.Interface;

public interface IClientConnected
{
    ClientInfo? GetClientInfo();
    ObjSocketSslStream? GetObjSocketSslStream();
    void SetClientInfo(ClientInfo? info);
    
    void Reset();

    ISocketWrapper? GetSocketActive();
}
