using System.Net.Sockets;

namespace LibSocketClient.Entities;

public class Listener
{
    public Socket Socket { get; set; } = new(AddressFamily.InterNetwork, 
        SocketType.Stream, ProtocolType.Tcp);
    
    public int Port { get;  set; }
    public bool Listening { get;  set; }
    
    public Socket? SocketClient { get; set; }
}
