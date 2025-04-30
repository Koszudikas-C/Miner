using System.Net.Security;
using System.Net.Sockets;
using LibCommunicationStatus;
using LibHandler.EventBus;
using LibRemoteAndClient.Entities.Remote.Client;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Interface;

namespace LibSsl.Service;

public class AuthSslClientService : IAuthSsl
{
    private readonly IAuthClient _authClient;
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;
    private readonly Dictionary<Guid, SslStream> _clientSslDict = new();

    public AuthSslClientService(IAuthClient authClient)
    {
        _authClient = authClient;
        _globalEventBusClient.Subscribe<ObjSocketSslStream>(OnSslAuthenticateClient);
    }

    public async Task AuthenticateAsync(ObjSocketSslStream objSocketSslStream,
        CancellationToken cts = default)
    {
        try
        {
            if (objSocketSslStream.Id == Guid.Empty)
                throw new ArgumentException("clientId cannot be empty");

            var sslStream = await _authClient.AuthenticateAsync(
                objSocketSslStream.SocketWrapper!, cts);
            
            _clientSslDict[objSocketSslStream.Id] = sslStream;
            
            OnSslAuthenticateClient(sslStream, objSocketSslStream.SocketWrapper!.InnerSocket, objSocketSslStream.Id);
        }
        catch (Exception ex)
        {
            Reconnect(objSocketSslStream.Id);
            throw new Exception(ex.Message);
        }
    }

    public void Reconnect(Guid clientId)
    {
        if (!_clientSslDict.TryGetValue(clientId, out var sslStream)) return;

        sslStream.Close();
        _clientSslDict.Remove(clientId);
    }
    
    private void OnSslAuthenticateClient(SslStream sslStream, Socket socket,
        Guid clientId)
    {
        var clientInfo = new ClientInfo()
        {
            Id = clientId,
            SocketWrapper = new SocketWrapper(socket),
            SslStreamWrapper = new SslStreamWrapper(sslStream)
        };
        
        _globalEventBusClient.Publish(clientInfo);
        CommunicationStatus.SetSending(false);
        CommunicationStatus.SetAuthenticated(true);
    }

    private void OnSslAuthenticateClient(ObjSocketSslStream objSocketSslStream)
    {
        _ = AuthenticateAsync(objSocketSslStream);
    }
}