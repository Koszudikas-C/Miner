using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using LibCommunicationStateRemote.Entities;
using LibSocket.Entities;
using LibSocketAndSslStreamRemote.Entities.Enum;
using LibSocketAndSslStreamRemote.Interface;
using Microsoft.Extensions.Logging;

namespace LibSocketRemote.Service;

/// <summary>
/// Do Bind is a reserve amount of socket for the kernel.
/// Accepts client connection triggers an event by passing the socket to class called. 
/// </summary>
/// <param name="logger"></param>
public class ListenerService(ILogger<ListenerService> logger) : IListener
{
    private readonly Listener _listener = new Listener();
    private readonly ILogger<ListenerService> _logger = logger;
    public event Func<Socket, CancellationToken, Task>? ConnectedActAsync;

    public async Task StartAsync(TypeAuthMode typeAuthMode, uint port, int maxConnections = 0,
        CancellationToken cts = default)
    {
        if (_listener.Listening) return;

        _listener.Port = (int)port;

        if (IsPortInUse(_listener.Port))
            throw new InvalidOperationException($"There is already a process using the door {_listener.Port}");

        _listener.Socket.Bind(new IPEndPoint(IPAddress.Any, _listener.Port));
        _listener.Socket.Listen(maxConnections);

        _logger.LogInformation($"Remote started successfully at the port {port} " +
                               $"with the kernel listening to the total of {maxConnections} ");

        _listener.Listening = true;
        
        CommunicationStateReceiveAndSend.SetConnecting(true);
        await ConnectForClientAsync(typeAuthMode, cts);
    }

    private static bool IsPortInUse(int port)
    {
        var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();

        return tcpConnInfoArray.Any(info => info.Port == port);
    }

    private async Task ConnectForClientAsync(TypeAuthMode typeAuthMode,
        CancellationToken cts)
    {
        _logger.LogInformation($"Starting client Acceptance mode {typeAuthMode}");

        while (!cts.IsCancellationRequested)
        {
            try
            {
                if (!_listener.Listening) break;

                _listener.SocketClient = await _listener.Socket.AcceptAsync(cts);

                OnConnectedActAsync(cts).ConfigureAwait(false).GetAwaiter();
            }
            catch (Exception e)
            {
                _logger.LogCritical("A serious error occurred when accepting a new client. Error: {Message}", e);
                _listener.SocketClient!.Close();
                throw new Exception();
            }
        }
    }

    public void Stop()
    {
        if (!_listener.Listening) return;

        _listener.Listening = false;
        _listener.Socket.Dispose();
    }

    private async Task OnConnectedActAsync(CancellationToken cts = default)
    {
        CheckNullSocketClient();
        SetConfigSocket();
        
        if (ConnectedActAsync is not null)
            await ConnectedActAsync.Invoke(_listener.SocketClient!, cts);
    }
    
    private void SetConfigSocket()
    {
        CheckNullSocketClient();
        _listener.SocketClient!.ReceiveTimeout = 10000;
        _listener.SocketClient.SendTimeout = 10000;
    }
    
    private void CheckNullSocketClient()
    {
        if (_listener.SocketClient is null)
        {
            throw new InvalidOperationException("Check the customer's " +
                                                "socket is passing with a null value");
        }
    }
}