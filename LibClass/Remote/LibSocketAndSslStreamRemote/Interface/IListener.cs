using System.Net.Sockets;
using LibSocketAndSslStreamRemote.Entities.Enum;

namespace LibSocketAndSslStreamRemote.Interface;

public interface IListener
{
    Task StartAsync(TypeAuthMode typeAuthMode, uint port, 
        int maxConnections = 0, CancellationToken cts = default);
    
    Task ReconnectAsync(TypeAuthMode typeAuthMode, uint port,
        int maxConnection, CancellationToken cts = default);
    
    void Stop();
    
    event Action<Socket> ConnectedAct;
}
