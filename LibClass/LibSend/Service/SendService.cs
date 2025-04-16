using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSend.Entities;
using LibSend.Interface;
using LibSsl.Entities;

namespace LibSend.Service;

public class SendService<T> : ISend<T>
{
    public async Task SendAsync(T data, ClientInfo clientInfo,
        TypeSocketSsl typeSocketSsl, CancellationToken cts = default)
    {
        switch (typeSocketSsl)
        {
            case TypeSocketSsl.SslStream:
                await SendAuth(data, clientInfo, cts);
                break;
            case TypeSocketSsl.Socket:
                await Send(data, clientInfo, cts);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(typeSocketSsl), typeSocketSsl, null);
        }
    }
    
    private static async Task SendAuth(T data, ClientInfo clientInfo,
        CancellationToken cts = default)
    {
        var send = new SendAuth<T>(clientInfo.SslStream!);
        
        await send.SendAsync(data, cts);
    }

    private static async Task Send(T data, ClientInfo clientInfo, 
        CancellationToken cts = default)
    {
        var send = new Send<T>(clientInfo.Socket!);
        
        await send.SendAsync(data, cts);
    }
    
    public async Task SendListAsync(List<T> dataList, 
        ClientInfo clientInfo, TypeSocketSsl typeSocketSsl, CancellationToken cts = default)
    {
        switch (typeSocketSsl)
        {
            case TypeSocketSsl.SslStream:
                await SendAuthList(dataList, clientInfo, cts);
                break;
            case TypeSocketSsl.Socket:
                await SendList(dataList, clientInfo, cts);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(typeSocketSsl), typeSocketSsl, null);
        }
    }
    
    private static async Task SendAuthList(List<T> dataList, ClientInfo clientInfo,
        CancellationToken cts = default)
    {
        var send = new SendAuth<T>(clientInfo.SslStream!);

        await send!.SendListAsync(dataList, cts);
    }
    
    private static async Task SendList(List<T> dataList, ClientInfo clientInfo,
        CancellationToken cts = default)
    {
        var send = new Send<T>(clientInfo.Socket!);

        await send!.SendListAsync(dataList, cts);
    }
}