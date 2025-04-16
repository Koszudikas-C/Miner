using System.Net.Security;
using System.Net.Sockets;

namespace LibSsl.Entities;

public class ObjSocketSslStream
{
    public Guid Id { get; set; }
    public Socket Socket { get; set; }
    public SslStream SslStream { get; set; }
}