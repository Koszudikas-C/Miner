using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using LibDto.Dto;
using LibHandler.EventBus;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSend.Interface;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Interface;

namespace LibSsl.Service;

public class AuthSslClientService : IAuthSsl
{
    private readonly IAuthClient _authClient;
    private readonly IReceive _receive;
    private readonly ISend<HttpStatusCode> _sendStatusCode;
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance;
    private readonly Dictionary<Guid, SslStream> _clientSslDict = new();

    public AuthSslClientService(IAuthClient authClient, IReceive receive,
        ISend<HttpStatusCode> sendStatusCode)
    {
        _authClient = authClient;
        _receive = receive;
        _sendStatusCode = sendStatusCode;
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        _globalEventBusClient.Subscribe<ObjSocketSslStream>((obj) =>
            _ = OnSslAuthenticateClient(obj));
    }

    public async Task AuthenticateAsync(ObjSocketSslStream objSocketSslStream,
        CancellationToken cts = default)
    {
        try
        {
            var sslStream = await _authClient.AuthenticateAsync(
                objSocketSslStream.SocketWrapper!, cts);

            _clientSslDict[objSocketSslStream.Id] = sslStream;

            var clientInfo = GetClientInfo(sslStream, objSocketSslStream.SocketWrapper!.InnerSocket);

            await ReceiveNonceAsync(clientInfo, cts);
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


    private async Task ReceiveNonceAsync(ClientInfo clientInfo, CancellationToken cts = default)
    {
        try
        {
            var task = new TaskCompletionSource<GuidTokenAuthDto>();

            void Handler(GuidTokenAuthDto guidTokenAuth)
            {
                task.TrySetResult(guidTokenAuth);
                
                _ = SendStatusCodeAsync(HttpStatusCode.OK, clientInfo);
                
                _globalEventBusClient.Publish(clientInfo, guidTokenAuth);
                _globalEventBusClient.Unsubscribe<GuidTokenAuthDto>(Handler);
            }

            _globalEventBusClient.Subscribe<GuidTokenAuthDto>(Handler);

            await _receive.ReceiveDataAsync(clientInfo, TypeSocketSsl.SslStream, 0, cts);
            
            await task.Task.WaitAsync(TimeSpan.FromSeconds(5), cts);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    private static ClientInfo GetClientInfo(SslStream sslStream, Socket socket)
    {
        return new ClientInfo
        {
            SocketWrapper = new SocketWrapper(socket),
            SslStreamWrapper = new SslStreamWrapper(sslStream)
        };
    }

    private async Task SendStatusCodeAsync(HttpStatusCode statusCode, ClientInfo clientInfo) =>
        await _sendStatusCode.SendAsync(statusCode, clientInfo, TypeSocketSsl.SslStream);


    private async Task OnSslAuthenticateClient(ObjSocketSslStream objSocketSslStream)
    {
        await AuthenticateAsync(objSocketSslStream);
    }
}