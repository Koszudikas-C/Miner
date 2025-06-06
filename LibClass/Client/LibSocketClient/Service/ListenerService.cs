using System.Net;
using System.Net.Sockets;
using LibSocketAndSslStreamClient.Entities;
using LibSocketAndSslStreamClient.Entities.Enum;
using LibSocketAndSslStreamClient.Interface;
using LibSocketClient.Entities;
using LibSocks5Client.Interface;

namespace LibSocketClient.Service;

public class ListenerService(
  IConfigVariable configVariable,
  ISocks5Options socks5Options,
  ISocks5 socks5)
  : IListener
{
  private readonly Listener _listener = new();

  public event Func<Socket, CancellationToken, Task>? ConnectedAct;

  public async Task StartAsync(TypeAuthMode typeAuthMode, uint port,
    CancellationToken cts = default)
  {
    if (_listener.Listening) return;

    _listener.Port = (int)port;

    Console.WriteLine($"Id do thread de entrada StartAsync: {Environment.CurrentManagedThreadId}");
    await ConnectWithRetryAsync(typeAuthMode, cts);
  }

  private async Task ConnectWithRetryAsync(TypeAuthMode typeAuthMode, CancellationToken cts)
  {
    var resultConfigVariable = configVariable.GetConfigVariable();
    var data = (ConfigVariable)resultConfigVariable.GetData();
    var ip = await Dns.GetHostEntryAsync(data.RemoteSslBlock!, cts);

    while (!cts.IsCancellationRequested)
    {
      try
      {
        if (typeAuthMode == TypeAuthMode.AllowAnonymous)
        {
          var resultConfigSocks5 = socks5Options.GetSocks5Options();

          Console.WriteLine($"trying to connect to the onio server {resultConfigSocks5.DestinationHost}");

          await socks5.ConnectAsync(() => _listener.Socket, resultConfigSocks5, cts);
        }
        else
        {
          Console.WriteLine($"trying to connect to the ssl server " +
                            $"{data.RemoteSslBlock}: {data.RemoteSslBlockPort}");

          Console.WriteLine($"Id do thread de entrada ConnectWithRetryAsync: {Environment.CurrentManagedThreadId}");
          await _listener.Socket.ConnectAsync(ip.AddressList[0].ToString(),
          data.RemoteSslBlockPort, cts);
        }
        Console.WriteLine("Connected to the server");
        _listener.Listening = true;
        
        await OnConnectedAct(_listener.Socket, cts);
        Console.WriteLine($"Id de saida do ConnectiWithRetryAsync: {Environment.CurrentManagedThreadId}");
        break;
      }
      catch (SocketException)
      {
        Console.WriteLine("Connection error trying again in 5 seconds");
        await Task.Delay(5000, cts);
      }
    }
  }

  public async Task ReconnectAsync(TypeAuthMode typeAuthMode, uint port,
    CancellationToken cts = default)
  {
    if (!_listener.Listening) return;

    _listener.Listening = false;
    _listener.Socket.Close();
    _listener.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    await StartAsync(typeAuthMode, port, cts);
  }

  public void Stop()
  {
    if (!_listener.Listening) return;

    _listener.Listening = false;
    _listener.Socket.Close();
  }

  private async Task OnConnectedAct(Socket socket, CancellationToken cts)
  {
    if (ConnectedAct is not null)
      await ConnectedAct.Invoke(socket, cts);
  }
}
