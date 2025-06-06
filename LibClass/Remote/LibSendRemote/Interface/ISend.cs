using LibEntitiesRemote.Entities;
using LibEntitiesRemote.Entities.Enum;

namespace LibSendRemote.Interface;

public interface ISend<T>
{
    Task SendAsync(T data, ClientInfo clientInfo,
        TypeSocketSsl typeSocketSsl, CancellationToken cts = default);
    
    Task SendListAsync(List<T> dataList, ClientInfo clientInfo,
        TypeSocketSsl typeSocketSsl, CancellationToken cts = default);

    Task SendFileAsync(T dataFile, ClientInfo clientInfo,
        TypeSocketSsl typeSocketSsl, CancellationToken cts = default);
}
