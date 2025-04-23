using System.Net.Sockets;
using LibSocketAndSslStream.Entities.Enum;
using LibSocks5.Service;

namespace LibSocket.Connection;

public class ListenerClient(
    AddressFamily addressFamily,
    SocketType socketType,
    ProtocolType protocolType) : ListenerBase(addressFamily, socketType, protocolType)
{
    public override async Task StartAsync(TypeAuthMode typeAuthMode, uint port,
        int maxConnections = 0, CancellationToken cts = default)
    {
        if (Listening) return;

        Port = (int)port;

        await ConnectWithRetryAsync(typeAuthMode, cts);
    }

    private async Task ConnectWithRetryAsync(TypeAuthMode typeAuthMode, CancellationToken cts)
    {
        var configVariable = GetIpAddress();
        while (!cts.IsCancellationRequested)
        {
            try
            {
                if (typeAuthMode == TypeAuthMode.AllowAnonymous)
                {
                    Console.WriteLine($"trying to connect to the onio server {Socks5Options!.DestinationHost}");
                    await Socks5Service.ConnectAsync(() => Socket, Socks5Options!, cts);
                }
                else
                {
                    Console.WriteLine($"trying to connect to the ssl server {configVariable.RemoteSslHost}: {configVariable.RemoteSslPort}");
                    await Socket.ConnectAsync(configVariable.RemoteSslHost, configVariable.RemoteSslPort, cts);
                }

                Console.WriteLine("Connected to the server");
                Listening = true;
                OnConnectedAct(Socket);
                break;
            }
            catch (SocketException)
            {
                Console.WriteLine("Connection error trying again in 5 seconds");
                await Task.Delay(5000, cts);
            }
        }
    }


    public override async Task ReconnectAsync(TypeAuthMode typeAuthMode, uint port,
        int maxConnection, CancellationToken cts = default)
    {
        if (!Listening) return;

        Listening = false;
        Socket.Close();
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        await StartAsync(typeAuthMode, port, maxConnection, cts);
    }

    public override void Stop()
    {
        if (!Listening) return;

        Listening = false;
        Socket.Close();
    }
}