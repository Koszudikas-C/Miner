using System.Net.Sockets;
using LibSocks5Remote.Entities;

namespace LibSocks5Remote.Interface;

public interface ISocks5
{
    Task<Socket> ConnectAsync(Func<Socket> socketFactory, Socks5Options options,
        CancellationToken cts = default);
}
