using System.Net;
using System.Net.Sockets;
using LibEntitiesClient.Interface;

namespace LibEntitiesClient.Entities;

public class SocketWrapper(Socket socket) : ISocketWrapper
{
    public bool Connected => socket.Connected;
    public string? RemoteEndPoint => ((IPEndPoint)socket.RemoteEndPoint!).Address.ToString();
    public string? LocalEndPoint => socket.LocalEndPoint!.ToString();
    public int PortRemote => ((IPEndPoint)socket.RemoteEndPoint!).Port;
    public Socket InnerSocket => socket;
}
