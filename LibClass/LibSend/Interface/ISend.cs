
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;

namespace LibSend.Interface;

public interface ISend<T>
{
    Task SendAsync(T data, ClientInfo clientInfo,
        TypeSocketSsl typeSocketSsl, CancellationToken cts = default);
    
    Task SendListAsync(List<T> dataList, ClientInfo clientInfo,
        TypeSocketSsl typeSocketSsl, CancellationToken cts = default);
}