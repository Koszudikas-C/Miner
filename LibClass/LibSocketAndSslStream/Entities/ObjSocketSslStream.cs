using System.Net.Security;
using System.Net.Sockets;
using LibSocket.Interface;

namespace LibSocket.Entities;

public class ObjSocketSslStream
{
    public Guid Id { get; set; }
    public ISocketWrapper? SocketWrapper { get; set; }
    public SslStream? SslStream { get; set; }
}