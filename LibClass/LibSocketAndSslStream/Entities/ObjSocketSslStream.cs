using System.Net.Security;
using LibSocketAndSslStream.Interface;

namespace LibSocketAndSslStream.Entities;

public class ObjSocketSslStream
{
    public Guid Id { get; set; }
    public ISocketWrapper? SocketWrapper { get; set; }
    public ISslStreamWrapper? SslStream { get; set; }
}