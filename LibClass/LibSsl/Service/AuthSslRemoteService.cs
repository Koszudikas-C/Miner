using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using LibDto.Dto;
using LibHandler.EventBus;
using LibMapperObj.Interface;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSend.Interface;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Interface;

namespace LibSsl.Service;

public class AuthSslRemoteService : IAuthSsl
{
    private readonly IAuthRemote _authRemote;
    private readonly IReceive _receive;
    private readonly ISend<GuidTokenAuthDto> _sendGuidTokenDto;
    private readonly IMapperObj _mapperObj;
    private static ClientInfo? _clientInfoTemp;
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;
    private readonly Dictionary<Guid, SslStream> _remoteSslDict = new();

    public AuthSslRemoteService(IAuthRemote authRemote, IReceive receive,
        ISend<GuidTokenAuthDto> sendGuidTokenDto, IMapperObj mapperObj)
    {
        _authRemote = authRemote;
        _receive = receive;
        _sendGuidTokenDto = sendGuidTokenDto;
        _mapperObj = mapperObj;

        _globalEventBusRemote.Subscribe<ObjSocketSslStream>(OnSslAuthenticateClient);
    }

    private void Subscribe()
    {
        _globalEventBusRemote.Subscribe<HttpStatusCode>((statusCode)
            => _ = OnReceiveStatusCode(statusCode));
    }

    private void Unsubscribe()
    {
        _globalEventBusRemote.Subscribe<HttpStatusCode>((statusCode)
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
            SocketWrapper = new SocketWrapper(socket),
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

            _globalEventBusRemote.Publish(_clientInfoTemp);
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