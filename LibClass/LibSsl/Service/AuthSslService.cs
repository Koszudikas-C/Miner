using System.Net.Security;
using System.Net.Sockets;
using LibCommunicationStatus;
using LibHandler.EventBus;
using LibRemoteAndClient.Entities.Remote.Client;
using LibSocketAndSslStream.Entities.Enum;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Interface;
using LibSsl.Auth;

namespace LibSsl.Service;

public class AuthSslService : IAuthSsl
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;

    public AuthSslService()
    {
        _globalEventBusRemote.Subscribe<ObjSocketSslStream>((objSocketSslStream) =>
            _ = OnClientInfoRemote(objSocketSslStream));
        
        
        _globalEventBusClient.Subscribe<ObjSocketSslStream>((objSocketSslStream) =>
            _ = OnClientInfoClient(objSocketSslStream));
    }

    public async Task AuthenticateAsync(ISocketWrapper socketWrapper, TypeRemoteClient typeRemoteClient, 
        Guid clientId, CancellationToken cts = default)
    {
        try
        {
            if(clientId == Guid.Empty) throw new ArgumentException("clientId cannot be empty");
            
            switch (typeRemoteClient)
            {
                case TypeRemoteClient.Client:
                    var resultClient = new AuthClient(socketWrapper.InnerSocket);
                    if (await resultClient.AuthenticateAsync(cts))
                    {
                        OnSslAuthenticateClient(resultClient.SslStream!, socketWrapper.InnerSocket,
                            clientId);
                    }

                    break;
                case TypeRemoteClient.Remote:
                    var resultRemote = new AuthRemote(socketWrapper.InnerSocket);
                    if (await resultRemote.AuthenticateAsync(cts))
                    {
                        OnSslAuthenticateRemote(resultRemote.SslStream!, socketWrapper.InnerSocket,
                            clientId);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(TypeRemoteClient),
                        typeRemoteClient, null);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);  
        }
        finally
        {
            _semaphore.Release();
        }
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

    private async Task OnClientInfoRemote(ObjSocketSslStream objSocketSslStream)
    {
        await _semaphore.WaitAsync();
        await AuthenticateAsync(objSocketSslStream.SocketWrapper!, TypeRemoteClient.Remote,
            objSocketSslStream.Id);
    }
    
    private async Task OnClientInfoClient(ObjSocketSslStream objSocketSslStream)
    {
        await _semaphore.WaitAsync();
        await AuthenticateAsync(objSocketSslStream.SocketWrapper!, TypeRemoteClient.Client,
            objSocketSslStream.Id);
    }
}