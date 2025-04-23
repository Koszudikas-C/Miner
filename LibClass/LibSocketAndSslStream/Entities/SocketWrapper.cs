using System.Net.Sockets;
using LibSocket.Interface;

namespace LibSocket.Entities;

public class SocketWrapper(Socket socket) : ISocketWrapper
{
    public bool Connected => socket.Connected;
    public Socket InnerSocket => socket;
}