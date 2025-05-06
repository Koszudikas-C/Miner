using LibRemoteAndClient.Entities.Remote.Client;

namespace WorkClientBlockChain.Utils.Interface;

public interface IPosAuth
{
    Task SendClientMine(ClientInfo clientInfo);
    Task ReceiveDataCrypt(ClientInfo clientInfo);
}