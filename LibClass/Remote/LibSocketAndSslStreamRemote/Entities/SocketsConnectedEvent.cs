using System.Net.Sockets;

namespace LibSocketAndSslStreamRemote.Entities;

public class SocketsConnectedEvent(Socket socketClient)
{
    public Socket SocketClient { get; set; } = socketClient;
}