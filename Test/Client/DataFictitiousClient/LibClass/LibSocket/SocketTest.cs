using System.Net;
using System.Net.Sockets;

namespace DataFictitiousClient.LibClass.LibSocket;

public static class SocketTest
{
    private static Socket? _socket;
    private static Socket? _socketClient;

    public static Socket GetSocketConnected()
    {
        if (_socket is not null)
            return _socket;

        _socket = new Socket(
            AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        _socket.Bind(new IPEndPoint(IPAddress.Loopback, 3000));
        _socket.Listen(1);

        return _socket;
    }

    public static async Task<Socket> GetSocketClientConnected(CancellationToken cts = default)
    {
        if (_socket is null)
            _ = GetSocketConnected();
        if (_socketClient is not null)
            return _socketClient;

        await Task.WhenAny(ConnectedClient(cts), AcceptClient(cts));

        return _socketClient!;
    }

    public static void DisposableSockets()
    {
        if (_socket is null) return;
        _socket.Dispose();
        
        if (_socketClient is null) return;
        _socketClient.Dispose();
    }

    private static async Task ConnectedClient(CancellationToken cts = default)
    {
        using var socket = new Socket(
            AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        await socket.ConnectAsync(new IPEndPoint(IPAddress.Loopback, 3000), cts);
    }

    private static async Task AcceptClient(CancellationToken cts = default)
    {
        while (true)
        {
            _socketClient = await _socket!.AcceptAsync(cts);
            break;
        }
    }
}