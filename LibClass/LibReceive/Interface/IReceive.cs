
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;

namespace LibReceive.Interface;

public interface IReceive
{
    Task ReceiveDataAsync(ClientInfo clientInfo, TypeRemoteClient typeEnum, 
        int countReceive = 0, CancellationToken cts = default);
}
