using System.Net;
using System.Net.Sockets;
using LibDto.Dto;
using LibSocket.Entities;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Entities.Enum;
using LibSocketAndSslStream.Interface;
using LibSocks5.Interface;

namespace LibSocket.Service;

public class ListenerClientService(
    IConfigVariable configVariable,
    ISocks5Options socks5Options,
    ISocks5 socks5)
    : IListenerClient
{
    private readonly Listener _listener = new();
    public event Action<Socket>? ConnectedAct;  

    public async Task StartAsync(TypeAuthMode typeAuthMode, uint port,
        int maxConnections = 0, CancellationToken cts = default)
    {
        if (_listener.Listening) return;

        _listener.Port = (int)port;
    
        await ConnectWithRetryAsync(typeAuthMode, cts);
    }

    private async Task ConnectWithRetryAsync(TypeAuthMode typeAuthMode, CancellationToken cts)
    {
        var resultConfigVariable = configVariable.GetConfigVariable();
        var data = (ConfigVariable)resultConfigVariable.GetData();
        var resultConfigSocks5 = socks5Options.GetSocks5Options();
        
        while (!cts.IsCancellationRequested)
        {
            try
            {
                if (typeAuthMode == TypeAuthMode.AllowAnonymous)
                {
                    Console.WriteLine($"trying to connect to the onio server {resultConfigSocks5.DestinationHost}");
                    
                    await socks5.ConnectAsync(() => _listener.Socket, resultConfigSocks5, cts);
                }
                else
                {
                    Console.WriteLine($"trying to connect to the ssl server " +
                                      $"{data.RemoteSslBlock}: {data.RemoteSslBlockPort}");
                    
                    await _listener.Socket.ConnectAsync((await Dns.GetHostEntryAsync(data.RemoteSslBlock!, cts))
                        .AddressList[0].ToString(),
                        data.RemoteSslBlockPort, cts);
                }

                Console.WriteLine("Connected to the server");
                _listener.Listening = true;
                OnConnectedAct(_listener.Socket);
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
        int maxConnection, CancellationToken cts = default)
    {
        if (!_listener.Listening) return;

        _listener.Listening = false;
        _listener.Socket.Close();
        _listener.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        await StartAsync(typeAuthMode, port, maxConnection, cts);
    }

    public void Stop()
    {
        if (!_listener.Listening) return;

        _listener.Listening = false;
        _listener.Socket.Close();
    }

    private void OnConnectedAct(Socket socket) => ConnectedAct?.Invoke(socket);
}