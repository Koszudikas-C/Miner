
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;

namespace LibReceive.Interface;

public interface IReceive
{
    Task ReceiveDataAsync(ClientInfo clientInfo, TypeSocketSsl typeSocketSsl, 
        int countReceive = 0, CancellationToken cts = default);

    Task ReceiveDataFileAsync(ClientInfo clientInfo, TypeSocketSsl typeSocketSsl,
        int countReceive = 0, CancellationToken cts = default);
}
