using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using LibCommunicationStateClient.Entities.Enum;
using LibDtoClient.Dto;
using LibEntitiesClient.Entities;
using LibEntitiesClient.Entities.Enum;
using LibHandlerClient.Entities;
using LibReceiveClient.Interface;
using LibSendClient.Interface;
using LibSocketAndSslStreamClient.Interface;
using Microsoft.Extensions.Logging;

namespace LibSslClient.Service;

public class AuthSslService : IAuthSsl
{
    private readonly IAuth _authClient;
    private readonly IReceive _receive;
    private readonly ISend<HttpStatusCode> _sendStatusCode;
    private readonly ILogger<AuthSslService> _logger;
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;

    public AuthSslService(IAuth authClient, IReceive receive,
        ISend<HttpStatusCode> sendStatusCode, ILogger<AuthSslService> logger)
    {
        _authClient = authClient;
        _receive = receive;
        _sendStatusCode = sendStatusCode;
        _logger = logger;
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        _globalEventBus.SubscribeAsync<ObjSocketSslStream>(async (handle, cts) =>
        {
            await OnSslAuthenticateClient(handle, cts);
        });
    }

    public async Task AuthenticateAsync(ObjSocketSslStream objSocketSslStream,
        CancellationToken cts = default)
    {
        try
        {
            var sslStream = await _authClient.AuthenticateAsync(
                objSocketSslStream.SocketWrapper!, cts);
            
            var clientInfo = GetClientInfo(sslStream, objSocketSslStream.SocketWrapper!.InnerSocket);

            await ReceiveNonceAsync(clientInfo, cts);
        }
        catch (OperationCanceledException e)
        {
            _logger.LogWarning("Authentication operation exceeded with Remote. Error: {Message}", e);
            _globalEventBus.Publish(ConnectionStates.NoAuthenticated, cts);
            throw new OperationCanceledException();
        }
        catch (Exception)
        {
            _globalEventBus.Publish(ApplicationState.Restart, cts);
            throw new Exception();
        }
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

                _globalEventBus.Publish(clientInfo, guidTokenAuth);
                _globalEventBus.Unsubscribe<GuidTokenAuthDto>(Handler);
            }

            _globalEventBus.Subscribe<GuidTokenAuthDto>(Handler);

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


    private async Task OnSslAuthenticateClient(ObjSocketSslStream objSocketSslStream, CancellationToken cts)
    {
        await AuthenticateAsync(objSocketSslStream, cts);
    }

    private void TriggerEvent(CancellationToken cts = default)
    {
        _globalEventBus.Publish(ConnectionStates.Authenticated, cts);
        _globalEventBus.Publish(ConnectionStates.Connected, cts);
    }
}