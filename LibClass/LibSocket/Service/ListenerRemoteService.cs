using System.Net;
using System.Net.Sockets;
using LibCommunicationStatus;
using LibRemoteAndClient.Entities.Client;
using LibSocket.Entities;
using LibSocketAndSslStream.Entities.Enum;
using LibSocketAndSslStream.Interface;
using LibSocks5.Entities;

namespace LibSocket.Service;

public class ListenerRemoteService()
    : IListenerRemote
{

    private readonly SemaphoreSlim _semaphore = new (1);
    private readonly Listener _listener = new Listener();
    
    public event Action<Socket>? ConnectedAct;
    
    public async Task StartAsync(TypeAuthMode typeAuthMode, uint port, int maxConnections = 0,
        CancellationToken cts = default)
    {
        if (_listener.Listening) return;
        
        _listener.Port = (int)port;
        _listener.Socket.Bind(new IPEndPoint(IPAddress.Any, _listener.Port));
        _listener.Socket.Listen(maxConnections);
        
        _listener.Listening = true;
        
        CommunicationStatus.SetConnected(true);
        CommunicationStatus.SetConnecting(true);
        
        await ConnectForClientAsync(typeAuthMode, port, maxConnections, cts);
    }

    private async Task ConnectForClientAsync(TypeAuthMode typeAuthMode,
        uint port, int maxConnection, CancellationToken cts)
    {
        while (!cts.IsCancellationRequested)
        {
            try
            {
                if(!_listener.Listening) break;

                await _semaphore.WaitAsync(cts);

                _listener.SocketClient = await _listener.Socket.AcceptAsync(cts);

                OnConnectedAct(_listener.SocketClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when accepting customer: {ex.Message}");
                await Task.Delay(5000, cts);
                await ReconnectAsync(typeAuthMode, port, maxConnection, cts);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    public async Task ReconnectAsync(TypeAuthMode typeAuthMode, 
        uint port, int maxConnection, CancellationToken cts = default)
    {
        if (!_listener.Listening) return;

        _listener.Socket.Close();
        _listener.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _listener.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        _listener.Listening = false;
        await Task.Delay(1000, cts);
        await StartAsync(typeAuthMode, (uint)_listener.Port, 0, cts);
    }

    public void Stop()
    {
        if (!_listener.Listening) return;

        _listener.Listening = false;
        _listener.Socket.Dispose();
    }

    
    private void OnConnectedAct(Socket socket) => ConnectedAct?.Invoke(socket);
}
