using System.Net.Sockets;
using LibSocks5.Entities;

namespace LibSocks5.Interface;

public interface ISocks5
{
    Task<Socket> ConnectAsync(Func<Socket> socketFactory, Socks5Options options,
        CancellationToken cts = default);
}