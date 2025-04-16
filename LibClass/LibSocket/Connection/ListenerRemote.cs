using System.Net;
using System.Net.Sockets;
using LibSocket.Entities.Enum;

namespace LibSocket.Connection;

public class ListenerRemote(AddressFamily addressFamily,
 SocketType socketType, ProtocolType protocolType) : ListenerBase(addressFamily, socketType, protocolType)
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private Socket? SocketClient { get; set; }

    public override async Task StartAsync(TypeAuthMode typeAuthMode, uint port, int maxConnections = 0,
        CancellationToken cts = default)
    {
        if (Listening) return;

        Port = (int)port;
        Socket.Bind(new IPEndPoint(IPAddress.Any, Port));
        Socket.Listen(maxConnections);

        Listening = true;
        await ConnectForClientAsync(typeAuthMode, cts);
    }

    private async Task ConnectForClientAsync(TypeAuthMode typeAuthMode, CancellationToken cts)
    {
        while (!cts.IsCancellationRequested)
        {
            try
            {
                if(!Listening) break;

                await _semaphore.WaitAsync(cts);

                SocketClient = await Socket.AcceptAsync(cts);

                OnConnectedAct(SocketClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when accepting customer: {ex.Message}");
                await Task.Delay(5000, cts);
                await ReconnectAsync(typeAuthMode, cts);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    public override async Task ReconnectAsync(TypeAuthMode typeAuthMode, CancellationToken cts = default)
    {
        if (!Listening) return;

        Socket.Close();
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        Listening = false;
        await Task.Delay(1000, cts);
        await StartAsync(typeAuthMode, (uint)Port, 0, cts);
    }

    public override void Stop()
    {
        if (!Listening) return;

        Listening = false;
        Socket.Dispose();
    }
}
