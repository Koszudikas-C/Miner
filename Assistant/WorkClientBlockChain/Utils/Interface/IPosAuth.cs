using LibRemoteAndClient.Entities.Remote.Client;

namespace WorkClientBlockChain.Utils.Interface;

public interface IPosAuth
{
    Task ReceiveDataCrypt(ClientInfo clientInfo);
}