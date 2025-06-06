using LibEntitiesRemote.Entities;
using LibEntitiesRemote.Entities.Enum;

namespace LibReceiveRemote.Interface;

public interface IReceive
{
    Task ReceiveDataAsync(ClientInfo clientInfo, TypeSocketSsl typeSocketSsl, 
        int countReceive = 0, CancellationToken cts = default);

    Task ReceiveDataFileAsync(ClientInfo clientInfo, TypeSocketSsl typeSocketSsl,
        int countReceive = 0, CancellationToken cts = default);
}
