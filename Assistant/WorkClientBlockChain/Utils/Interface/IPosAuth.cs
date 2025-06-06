using LibEntitiesClient.Entities;

namespace WorkClientBlockChain.Utils.Interface;

public interface IPosAuth
{
    Task SendClientMine(ClientInfo clientInfo);
    Task ReceiveDataCrypt(ClientInfo clientInfo);
}
