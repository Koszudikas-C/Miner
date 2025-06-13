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
        _listener.Port = (int)port;
        await ConnectWithRetryAsync(typeAuthMode, cts);
    }

    public async Task ReconnectAsync(Socket socket, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        if (!socket.Connected) return;

        await socket.DisconnectAsync(true, cts);

        _listener.SocketClient = socket;
        await _listener.SocketClient.DisconnectAsync(true, cts);
        await ConnectWithRetryAsync(typeAuthMode, cts);
    }

    private async Task ConnectWithRetryAsync(TypeAuthMode typeAuthMode, CancellationToken cts)
    {
        CheckNullSocketClient();
        await (typeAuthMode == TypeAuthMode.RequireAuthentication
            ? ConnectDefaultRemoteAsync(cts)
            : ConnectSocks5RemoteAsync(cts));
    }

    private async Task ConnectDefaultRemoteAsync(CancellationToken cts = default)
    {
        var resultConfigVariable = configVariable.GetConfigVariable();
        var data = (ConfigVariable)resultConfigVariable.GetData();
        var ip = await Dns.GetHostEntryAsync(data.RemoteSslBlock!, cts);
        do
        {
            try
            {
                Console.WriteLine($"trying to connect to the ssl server " +
                                  $"{ip.AddressList[0].ToString()}: {data.RemoteSslBlockPort}");

                await _listener.SocketClient.ConnectAsync(ip.AddressList[0].ToString(),
                    data.RemoteSslBlockPort,
                    cts);

                Console.WriteLine("Connected to the server");
                _listener.Listening = true;

                await OnConnectedAct(cts);
                break;
            }
            catch (SocketException)
            {
                Console.WriteLine("Connection error trying again in 5 seconds");
                await Task.Delay(5000, cts);
            }
        } while (!cts.IsCancellationRequested);
    }

    private async Task ConnectSocks5RemoteAsync(CancellationToken cts = default)
    {
        do
        {
            try
            {
                var resultConfigSocks5 = socks5Options.GetSocks5Options();

                Console.WriteLine($"trying to connect to the onio server {resultConfigSocks5.DestinationHost}");

                await socks5.ConnectAsync(() => _listener.SocketClient, resultConfigSocks5, cts);

                await OnConnectedAct(cts);
                break;
            }
            catch (SocketException)
            {
                Console.WriteLine("Connection error trying again in 5 seconds");
                await Task.Delay(5000, cts);
            }
            catch (Exception)
            {
                Disposable();
                throw;
            }
        } while (!cts.IsCancellationRequested);
    }

    public void Disposable()
    {
        if (!_listener.Listening) return;

        _listener.Listening = false;
        _listener.SocketClient.Dispose();
    }

    private async Task OnConnectedAct(CancellationToken cts = default)
    {
        CheckNullSocketClient();
        SetConfigSocket();
        _listener.Listening = true;
        if (ConnectedAct is not null)
            await ConnectedAct.Invoke(_listener.SocketClient, cts);
    }

    private void CheckNullSocketClient()
    {
        if (_listener.SocketClient is null)
        {
            throw new InvalidOperationException("Check the customer's" +
                                                " socket is passing with a null value");
        }
    }

    private void SetConfigSocket()
    {
        CheckNullSocketClient();
        _listener.SocketClient.ReceiveTimeout = 10000;
        _listener.SocketClient.SendTimeout = 10000;
    }
}