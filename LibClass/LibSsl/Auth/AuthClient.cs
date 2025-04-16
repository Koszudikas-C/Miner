using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using LibTimeTask.Auth;
using LibCertificate;

namespace LibSsl.Auth;
public sealed class AuthClient(Socket socket) : AuthBase(socket)
{
    public override async Task<bool> AuthenticateAsync(CancellationToken cts = default)
    {
        SslStream = null;
        
        if (!Socket.Connected) return false;
        
        var networkStream = new NetworkStream(Socket);
        
        
         SslStream = new SslStream(networkStream, 
             false, ValidateCertificate.CertificateValidationCallBack!, 
             null);
         
         var authenticateTask = SslStream!.AuthenticateAsClientAsync(Host,
             null, SslProtocols.Tls12 
                   | SslProtocols.Tls13, true);
         
         await Task.WhenAny(authenticateTask, AuthTimeClient.AuthenticateClientTimeout);
        
         return authenticateTask.IsCompleted ? true :
             throw new AuthenticationException("Failed to authenticate client");
    }
}