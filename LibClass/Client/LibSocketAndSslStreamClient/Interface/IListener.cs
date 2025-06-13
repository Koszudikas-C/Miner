using System.Net.Sockets;
using LibSocketAndSslStreamClient.Entities.Enum;

namespace LibSocketAndSslStreamClient.Interface;

public interface IListener
{
  Task StartAsync(TypeAuthMode typeAuthMode, uint port,
    CancellationToken cts = default);
  
  Task ReconnectAsync(Socket socket , TypeAuthMode typeAuthMode,
    CancellationToken cts = default);
  
  void Disposable();
  
  event Func<Socket, CancellationToken, Task>? ConnectedAct;
}
