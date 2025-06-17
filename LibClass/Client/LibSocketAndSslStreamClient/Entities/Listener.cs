using System.Net.Sockets;
using LibSocketAndSslStreamClient.Interface;

namespace LibSocketAndSslStreamClient.Entities;

public class Listener   
{
    public Socket SocketClient { get; set; } = new (AddressFamily.InterNetwork, 
        SocketType.Stream, ProtocolType.Tcp);

    public int CountReconnection { get; set; }
    public int Port { get;  set; }
    public bool Listening { get;  set; }
}
