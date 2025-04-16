using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using LibCertificate;
using LibTimeTask.Auth;

namespace LibSsl.Auth;

public class AuthRemote(Socket socket) : AuthBase(socket)
{
    public override async Task<bool> AuthenticateAsync(CancellationToken cts = default)
    {
        SslStream = null;
        
        if (!Socket.Connected) return false;
        
        var networkStream = new NetworkStream(Socket);

        SslStream = new SslStream(networkStream, false);

        await SslStream.AuthenticateAsServerAsync(
            Certificate.LoadCertificate(), false,
            SslProtocols.Tls12 | SslProtocols.Tls13, true);
        
        return SslStream.IsAuthenticated ? true : 
            throw new AuthenticationException("Failed to authenticate client");
    }
}