using LibEntitiesClient.Entities;

namespace WorkClientBlockChain.Connection.Interface;

public interface IClientConnected
{
    ClientInfo? GetClientInfo();
    ObjSocketSslStream? GetObjSocketSslStream();
    void SetClientInfo(ClientInfo? info);
    
    void Reset();
}
