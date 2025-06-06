using LibEntitiesClient.Entities;
using LibEntitiesClient.Entities.Enum;

namespace LibReceiveClient.Interface;

public interface IReceive
{
    Task ReceiveDataAsync(ClientInfo clientInfo, TypeSocketSsl typeSocketSsl, 
        int countReceive = 0, CancellationToken cts = default);

    Task ReceiveDataFileAsync(ClientInfo clientInfo, TypeSocketSsl typeSocketSsl,
        int countReceive = 0, CancellationToken cts = default);
}
