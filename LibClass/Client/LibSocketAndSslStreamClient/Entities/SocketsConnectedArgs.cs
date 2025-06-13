using System.Net.Sockets;

namespace LibSocketAndSslStreamClient.Entities;

public class SocketsConnectedArgs(Socket socketClient) : EventArgs
{
    public Socket SocketClient { get; } = socketClient;
}