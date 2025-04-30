using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using LibSocketAndSslStream.Interface;
using LibCertificate.Util;
using LibSocketAndSslStream.Entities;
using LibTimeTask.Auth;

namespace LibSsl.Service;

public class AuthClientService(IConfigVariable configVariable) : IAuthClient
{
    public async Task<SslStream> AuthenticateAsync(ISocketWrapper socketWrapper,
        CancellationToken cts = default)
    {
        try
        {
            if (socketWrapper is null or {InnerSocket: null} )
                throw new ArgumentNullException(nameof(socketWrapper));
        
            if (!socketWrapper.Connected)
                throw new Exception("Socket is not connected");
        
            var networkStream = new NetworkStream(socketWrapper.InnerSocket);
        
            var sslStream = new SslStream(networkStream, false,
                ValidateCertificate.CertificateValidationCallBack!, null);
        
            var config = configVariable.GetConfigVariable();
            var data = (ConfigVariable)config.GetData();
            
            var authenticateTask = sslStream.AuthenticateAsClientAsync(data.RemoteSslBlock!,null,
                SslProtocols.Tls12 | SslProtocols.Tls13, true);
        
            await Task.WhenAny(authenticateTask, AuthTimeClient.AuthenticateClientTimeout);
        
            if (!authenticateTask.IsCompleted)
                throw new AuthenticationException("Failed to authenticate client");
        
            if (!sslStream.IsAuthenticated)
                throw new AuthenticationException("Failed to authenticate client");
        
            return sslStream;

        }
        catch (Exception ex)
        {
            throw new Exception("Failure when trying to authenticate with the server." +
                                $" Check the connection to the server/host. Error: {ex.Message}");
        }
    }
}
