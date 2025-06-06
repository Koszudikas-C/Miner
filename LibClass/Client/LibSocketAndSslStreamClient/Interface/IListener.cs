using System.Net.Sockets;
using LibSocketAndSslStreamClient.Entities.Enum;

namespace LibSocketAndSslStreamClient.Interface;

public interface IListener
{
  Task StartAsync(TypeAuthMode typeAuthMode, uint port,
    CancellationToken cts = default);
  
  Task ReconnectAsync(TypeAuthMode typeAuthMode, uint port,
    CancellationToken cts = default);
  
  void Stop();
  
  event Func<Socket, CancellationToken, Task>? ConnectedAct;
}
