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

            var task = sslStream.AuthenticateAsServerAsync(certificate.LoadCertificate(),
                false, SslProtocols.Tls12 | SslProtocols.Tls13,
                true);
            await WithTimeoutAndCancel(task, TimeSpan.FromSeconds(5));
            await task;
            return sslStream;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to try to authenticate the client via SSL/TLS." +
                                $"Check the connection to the client. Error: {ex.Message}");
        }
    }
    
    private static async Task WithTimeoutAndCancel(Task task, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource();
        var delay = Task.Delay(timeout, cts.Token);

        var completed = await Task.WhenAny(task, delay);
        if (completed == delay)
        {
            await cts.CancelAsync();
            throw new TimeoutException("Tempo limite excedido.");
        }
    }
}