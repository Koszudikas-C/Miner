using System.Net;
using System.Net.Sockets;
using WorkClientBlockChain.Utils.Interface;

namespace WorkClientBlockChain.Utils;

internal class PortOpen : IPortOpen
{
    public async Task<bool> IsOpenPortAsync(int port, CancellationToken cts = default)
    {
        using var tcpClient = new TcpClient();
        var connect = tcpClient.ConnectAsync(IPAddress.Any, port);
        var result = await Task.WhenAny(connect, Task.Delay(1000, cts));

        if (!result.IsCompleted) return false;
        
        tcpClient.Close();
        return true;
    }
}

