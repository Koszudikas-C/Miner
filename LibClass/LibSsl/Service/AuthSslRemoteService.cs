using System.Net.Security;
using System.Net.Sockets;
using LibHandler.EventBus;
using LibRemoteAndClient.Entities.Remote.Client;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Interface;

namespace LibSsl.Service;

public class AuthSslRemoteService : IAuthSsl
{
    private readonly IAuthRemote _authRemote;
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;
    private readonly Dictionary<Guid, SslStream> _remoteSslDict = new();

    public AuthSslRemoteService(IAuthRemote authRemote)
    {
        _authRemote = authRemote;
        _globalEventBusRemote.Subscribe<ObjSocketSslStream>(OnSslAuthenticateClient);
    }

    public async Task AuthenticateAsync(ObjSocketSslStream objSocketSslStream,
        CancellationToken cts = default)
    {
        try
        {
            if (objSocketSslStream.Id == Guid.Empty) 
                throw new ArgumentException("clientId cannot be empty");
            
            Reconnect(objSocketSslStream.Id);

            var sslStream = await _authRemote.AuthenticateAsync(objSocketSslStream.SocketWrapper!, cts);
        
            _remoteSslDict[objSocketSslStream.Id] = sslStream;
            
            OnSslAuthenticateRemote(sslStream, objSocketSslStream.SocketWrapper!.InnerSocket, objSocketSslStream.Id);
        }
        catch (Exception ex)
        {
            Reconnect(objSocketSslStream.Id);
            throw new Exception(ex.Message);
        }
    }
    
    public void Reconnect(Guid clientId)
    {
        if (!_remoteSslDict.TryGetValue(clientId, out var sslStream)) return;

        sslStream.Close();
        _remoteSslDict.Remove(clientId);
    }
    
    private void OnSslAuthenticateRemote(SslStream sslStream, 
        Socket socket, Guid clientId)
    {
        var clientInfo = new ClientInfo()
        {
            Id = clientId,
            SocketWrapper = new SocketWrapper(socket),
            SslStreamWrapper = new SslStreamWrapper(sslStream)
        };
        
        _globalEventBusRemote.Publish(clientInfo);
    }

    private void OnSslAuthenticateClient(ObjSocketSslStream objSocketSslStream)
    {
        _ = AuthenticateAsync(objSocketSslStream);
    }
}