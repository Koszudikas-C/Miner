using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSend.Entities;
using LibSend.Interface;

namespace LibSend.Service;

public class SendServiceClient<T> : ISend<T>
{
    public async Task SendAsync(T data, ClientInfo clientInfo,
        TypeSocketSsl typeSocketSsl, CancellationToken cts = default)
    {
        if (clientInfo == null)
            throw new ArgumentNullException(nameof(clientInfo), "ClientInfo cannot be null.");
        switch (typeSocketSsl)
        {
            case TypeSocketSsl.SslStream:
                await SendAuth(data, clientInfo, cts);
                break;
            case TypeSocketSsl.Socket:
                await Send(data, clientInfo, cts);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(typeSocketSsl), $"Invalid socket type: {typeSocketSsl}");
        }
    }

    private static async Task SendAuth(T data, ClientInfo clientInfo, CancellationToken cts = default)
    {
        if (clientInfo.SslStreamWrapper?.InnerSslStream == null)
            throw new InvalidOperationException("SslStream is not available in ClientInfo.");
        var send = new SendAuth<T>(clientInfo.SslStreamWrapper.InnerSslStream);
        await send.SendAsync(data, cts);
    }

    private static async Task Send(T data, ClientInfo clientInfo, CancellationToken cts = default)
    {
        if (clientInfo.SocketWrapper?.InnerSocket == null)
            throw new InvalidOperationException("Socket is not available in ClientInfo.");
        var send = new Send<T>(clientInfo.SocketWrapper.InnerSocket);
        await send.SendAsync(data, cts);
    }

    public async Task SendListAsync(List<T> dataList, ClientInfo clientInfo, TypeSocketSsl typeSocketSsl, CancellationToken cts = default)
    {
        if (clientInfo == null)
            throw new ArgumentNullException(nameof(clientInfo), "ClientInfo cannot be null.");
        switch (typeSocketSsl)
        {
            case TypeSocketSsl.SslStream:
                await SendAuthList(dataList, clientInfo, cts);
                break;
            case TypeSocketSsl.Socket:
                await SendList(dataList, clientInfo, cts);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(typeSocketSsl), $"Invalid socket type: {typeSocketSsl}");
        }
    }

    private static async Task SendAuthList(List<T> dataList, ClientInfo clientInfo, CancellationToken cts = default)
    {
        if (clientInfo.SslStreamWrapper?.InnerSslStream == null)
            throw new InvalidOperationException("SslStream is not available in ClientInfo.");
        var send = new SendAuth<T>(clientInfo.SslStreamWrapper.InnerSslStream);
        await send.SendListAsync(dataList, cts);
    }

    private static async Task SendList(List<T> dataList, ClientInfo clientInfo, CancellationToken cts = default)
    {
        if (clientInfo.SocketWrapper?.InnerSocket == null)
            throw new InvalidOperationException("Socket is not available in ClientInfo.");
        var send = new Send<T>(clientInfo.SocketWrapper.InnerSocket);
        await send.SendListAsync(dataList, cts);
    }
}
