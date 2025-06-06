using System.Net.Sockets;
using LibSocks5Client.Entities;

namespace LibSocks5Client.Interface;

public interface ISocks5
{
    Task<Socket> ConnectAsync(Func<Socket> socketFactory, Socks5Options options,
        CancellationToken cts = default);
}
