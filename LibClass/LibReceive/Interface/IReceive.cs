
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSsl.Entities;

namespace LibReceive.Interface;

public interface IReceive
{
    Task ReceiveDataAsync(ClientInfo clientInfo, TypeRemoteClient typeEnum, 
        int countReceive = 0, CancellationToken cts = default);
}
