using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using LibCertificateRemote.Interface;
using LibEntitiesRemote.Interface;
using LibSocketAndSslStreamRemote.Interface;
using LibTimeTaskRemote.Auth;

namespace LibSslRemote.Service;

public class AuthService(ICertificate certificate) : IAuth
{
  public async Task<SslStream> AuthenticateAsync(ISocketWrapper socketWrapper,
    CancellationToken cts = default)
  {
    try
    {
      using var ctsTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
      using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts, ctsTimeout.Token);
      
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

      var completedTask = await Task.WhenAny(task, AuthTime.AuthenticateRemoteTimeout(linkedCts.Token));

      if (completedTask == task) return sslStream;
    
      await linkedCts.CancelAsync();
      Console.WriteLine(completedTask.IsCanceled);
      throw new AuthenticationException("Waiting time for authentication were exceeded!.");
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      throw new Exception($"Failed to try to authenticate the client via SSL/TLS." +
                          $"Check the connection to the client. Error: {ex.Message}");
    }
  }
}
