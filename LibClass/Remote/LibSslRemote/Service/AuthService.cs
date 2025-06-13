using System.Net.Sockets;
using System.Net.Security;
using LibCertificateRemote.Interface;
using LibCommunicationStateRemote.Entities;
using LibCommunicationStatusRemote.Entities.Enum;
using LibEntitiesRemote.Interface;
using LibSocketAndSslStreamRemote.Interface;
using LibSslRemote.Entities;

namespace LibSslRemote.Service;

public class AuthService(ISslServerAuthOptions sslServerAuthOptions) : IAuth
{
    public async Task<SslStream> AuthenticateAsync(ISocketWrapper socketWrapper,
        CancellationToken cts = default)
    {

            using var ctsTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts, ctsTimeout.Token);

            if (socketWrapper is null or { InnerSocket: null })
                throw new ArgumentNullException(nameof(socketWrapper));

            if (!socketWrapper.InnerSocket.Connected)
                throw new SocketException((int)SocketError.NotConnected);

            var sslOptions = sslServerAuthOptions.GetConfigSslServerAuthenticationOptions();
            
            var networkStream = new NetworkStream(socketWrapper.InnerSocket);
            var sslStream = new SslStream(networkStream, false);

            try
            {
                await sslStream.AuthenticateAsServerAsync(sslOptions, linkedCts.Token);
                return SetConfigSslStream(sslStream);
            }
            catch (OperationCanceledException e)
            {
                sslStream.Close();
                throw new OperationCanceledException(e.Message);
            }
    }

    private static SslStream SetConfigSslStream(SslStream sslStream)
    {
        sslStream.ReadTimeout = 10000;
        sslStream.WriteTimeout = 10000;
        return sslStream;
    }
}