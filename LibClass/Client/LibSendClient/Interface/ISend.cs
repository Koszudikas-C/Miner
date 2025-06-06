using LibEntitiesClient.Entities;
using LibEntitiesClient.Entities.Enum;

namespace LibSendClient.Interface;

public interface ISend<T>
{
    Task SendAsync(T data, ClientInfo clientInfo,
        TypeSocketSsl typeSocketSsl, CancellationToken cts = default);
    
    Task SendListAsync(List<T> dataList, ClientInfo clientInfo,
        TypeSocketSsl typeSocketSsl, CancellationToken cts = default);

    Task SendFileAsync(T dataFile, ClientInfo clientInfo,
        TypeSocketSsl typeSocketSsl, CancellationToken cts = default);
}
