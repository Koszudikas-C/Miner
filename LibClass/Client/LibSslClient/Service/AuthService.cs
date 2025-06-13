using System.Net.Sockets;
using System.Net.Security;
using LibEntitiesClient.Interface;
using LibSocketAndSslStreamClient.Interface;
using LibUtilClient.Util;
using LibSslClient.Entities.Enum;

namespace LibSslClient.Service;

public class AuthService(IConfigVariable configVariable) : IAuth
{
    public async Task<SslStream> AuthenticateAsync(ISocketWrapper socketWrapper,
        CancellationToken cts = default)
    {
        using var ctsSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var networkStream = CreateNetworkStream(socketWrapper);

        var sslStream = CreateSslStream(networkStream);
        try
        {
            CheckParams(socketWrapper);

            var sslClientOptions = GetConfigSslClientAuthenticationOptions();

            await sslStream.AuthenticateAsClientAsync(sslClientOptions, ctsSource.Token);

            return SetConfigSslStream(sslStream);
        }
        catch (OperationCanceledException)
        {
            sslStream.Close();
            throw;
        }
    }

    private static void CheckParams(ISocketWrapper socketWrapper)
    {
        ArgumentNullException.ThrowIfNull(socketWrapper);

        if (socketWrapper.InnerSocket is null)
            throw new InvalidOperationException("SocketWrapper does not contain a valid socket.");

        if (!socketWrapper.Connected)
            throw new InvalidOperationException("Socket is not connected.");
    }


    private static NetworkStream CreateNetworkStream(ISocketWrapper socketWrapper)
    {
        var networkStream = new NetworkStream(socketWrapper.InnerSocket);
        return networkStream;
    }

    private static SslStream CreateSslStream(NetworkStream networkStream)
    {
        var sslStream = new SslStream(networkStream, false,
            ValidateCertificate.CertificateValidationCallBack!, null);
        return sslStream;
    }

    private SslClientAuthenticationOptions GetConfigSslClientAuthenticationOptions()
    {
        var sslClientAuth = new SslClientAuth(configVariable);

        return sslClientAuth.GetConfigSslClientAuthenticationOptions();
    }

    private static SslStream SetConfigSslStream(SslStream sslStream)
    {
        sslStream.ReadTimeout = 10000;
        sslStream.WriteTimeout = 10000;
        
        return sslStream;
    }
}