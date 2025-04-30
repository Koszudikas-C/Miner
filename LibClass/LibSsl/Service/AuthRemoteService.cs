using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using LibSocketAndSslStream.Interface;
using LibCertificate;
using LibCertificate.Interface;

namespace LibSsl.Service;

public class AuthRemoteService(ICertificate certificate) : IAuthRemote
{
    public async Task<SslStream> AuthenticateAsync(ISocketWrapper socketWrapper,
        CancellationToken cts = default)
    {
        try
        {
            if (socketWrapper is null or { InnerSocket: null })
                throw new ArgumentNullException(nameof(socketWrapper));

            if (!socketWrapper.Connected)
                throw new Exception("Socket is not connected");

            var networkStream = new NetworkStream(socketWrapper.InnerSocket);
            var sslStream = new SslStream(networkStream, false, 
                null, null);

            await sslStream.AuthenticateAsServerAsync(certificate.LoadCertificate(),
                false, SslProtocols.Tls12 | SslProtocols.Tls13,
                true);
            
            if (!sslStream.IsAuthenticated)
                throw new AuthenticationException("Failed to authenticate remote");

            return sslStream;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to try to authenticate the client via SSL/TLS." +
                                $"Check the connection to the client. Error: {ex.Message}");
        }
    }
}