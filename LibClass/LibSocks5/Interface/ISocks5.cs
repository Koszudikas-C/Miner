using System.Net.Sockets;
using LibSocks5.Entities;

namespace LibSocks5.Interface;

public interface ISocks5
{
    public interface ISocks5
    {
        Task<Socket> Connect(Func<Socket> socketFactory, Socks5Options options, 
            CancellationToken cancellationToken = default);
    }
}