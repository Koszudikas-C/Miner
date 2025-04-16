using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace LibSsl.Auth;

public abstract class AuthBase(Socket socket)
{
    protected const string Host = "monerokoszudikas.duckdns.org";
    
    public Guid Id { get; protected set; }
    protected Socket Socket { get; set; } = socket;
    public SslStream? SslStream { get; protected set; }
    public abstract Task<bool> AuthenticateAsync(CancellationToken cts = default);
}