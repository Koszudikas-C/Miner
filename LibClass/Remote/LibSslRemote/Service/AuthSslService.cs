using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using LibDtoRemote.Dto;
using LibEntitiesRemote.Entities;
using LibEntitiesRemote.Entities.Enum;
using LibHandlerRemote.Entities;
using LibMapperObjRemote.Interface;
using LibReceiveRemote.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibSendRemote.Interface;
using LibSocketAndSslStreamRemote.Interface;

namespace LibSslRemote.Service;

public class AuthSslService : IAuthSsl
{
    private readonly IAuth _authRemote;
    private readonly IReceive _receive;
    private readonly ISend<GuidTokenAuthDto> _sendGuidTokenDto;
    private readonly IMapperObj _mapperObj;
    private static ClientInfo? _clientInfoTemp;
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance!;
    private readonly Dictionary<Guid, SslStream> _remoteSslDict = new();

    public AuthSslService(IAuth authRemote, IReceive receive,
        ISend<GuidTokenAuthDto> sendGuidTokenDto, IMapperObj mapperObj)
    {
        _authRemote = authRemote;
        _receive = receive;
        _sendGuidTokenDto = sendGuidTokenDto;
        _mapperObj = mapperObj;

        _globalEventBus.Subscribe<ObjSocketSslStream>(OnSslAuthenticateClient);
    }

    private void Subscribe()
    {
        _globalEventBus.Subscribe<HttpStatusCode>((statusCode)
            => _ = OnReceiveStatusCode(statusCode));
    }

    private void Unsubscribe()
    {
        _globalEventBus.Subscribe<HttpStatusCode>((statusCode)
            => _ = OnReceiveStatusCode(statusCode));
    }

    public async Task AuthenticateAsync(ObjSocketSslStream objSocketSslStream,
        CancellationToken cts = default)
    {
        try
        {
            var sslStream = await _authRemote.AuthenticateAsync(objSocketSslStream.SocketWrapper!, cts);

            var clientInfo = OnSslAuthenticateRemote(sslStream, objSocketSslStream.SocketWrapper!.InnerSocket,
                objSocketSslStream.Id);

            _clientInfoTemp = clientInfo;
            _remoteSslDict[objSocketSslStream.Id] = sslStream;

            await SendGuidTokenNonce(clientInfo, cts);
        }
        catch (Exception)
        {
            Reconnect(objSocketSslStream.Id);
            throw;
        }
    }

    public void Reconnect(Guid clientId)
    {
        if (!_remoteSslDict.TryGetValue(clientId, out var sslStream)) return;

        sslStream.Close();
        _remoteSslDict.Remove(clientId);
    }

    private static ClientInfo OnSslAuthenticateRemote(SslStream sslStream,
        Socket socket, Guid clientId) =>
        new ClientInfo
        {
            Id = clientId,
            SocketWrapper  = new SocketWrapper(socket),
            SslStreamWrapper = new SslStreamWrapper(sslStream)
        };


    private async Task SendGuidTokenNonce(ClientInfo clientInfo, CancellationToken cts = default)
    {
        try
        {
            var guidTokenAuthDto = _mapperObj.MapToDto(new GuidTokenAuth(), new GuidTokenAuthDto());

            _clientInfoTemp!.Id = guidTokenAuthDto.GuidTokenGlobal;

            await _sendGuidTokenDto.SendAsync(guidTokenAuthDto, clientInfo, TypeSocketSsl.SslStream, cts);

            Subscribe();
            await _receive.ReceiveDataAsync(clientInfo, TypeSocketSsl.SslStream, 0, cts);
        }
        catch (Exception)
        {
            DisconnectClient(clientInfo);
            throw new Exception("It was not possible to send the token to the client.");
        }
    }

    private async Task ReceiveClientHandshakeRequestDtoAsync(CancellationToken cts = default)
    {
        try
        {
            await _receive.ReceiveDataAsync(_clientInfoTemp!, TypeSocketSsl.SslStream, 0, cts);

            _globalEventBus.Publish(_clientInfoTemp);
            Unsubscribe();
        }
        catch (Exception)
        {
            DisconnectClient(_clientInfoTemp!);
            throw;
        }
    }

    private static void DisconnectClient(ClientInfo clientInfo) => clientInfo.Disconnect();

    private void OnSslAuthenticateClient(ObjSocketSslStream objSocketSslStream)
    {
        _ = AuthenticateAsync(objSocketSslStream);
    }

    private async Task OnReceiveStatusCode(HttpStatusCode httpStatusCode)
    {
        if (_clientInfoTemp is null)
            throw new Exception(
                "The client is not authenticated. Please authenticate the client first.");

        if (httpStatusCode == HttpStatusCode.OK)
        {
            await ReceiveClientHandshakeRequestDtoAsync();
            return;
        }

        _clientInfoTemp.Disconnect();
        throw new Exception(
            $"There was a problem with customer authentication. Returned status type: {httpStatusCode}");
    }
}
