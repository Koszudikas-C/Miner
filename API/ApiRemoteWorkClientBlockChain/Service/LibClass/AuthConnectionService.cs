using System.Net;
using System.Security.Cryptography;
using System.Text;
using ApiRemoteWorkClientBlockChain.Entities;
using ApiRemoteWorkClientBlockChain.Interface.Repository;
using LibAuthSecurityConnectionRemote.Interface;
using LibDtoRemote.Dto;
using LibEntitiesRemote.Entities;
using LibEntitiesRemote.Entities.Enum;
using LibHandlerRemote.Entities;
using LibMapperObjRemote.Interface;
using LibSendRemote.Interface;
using NuGet.Packaging;

namespace ApiRemoteWorkClientBlockChain.Service.LibClass;

public class AuthConnectionService : IAuthConnection
{
  private readonly ISend<HttpStatusCode> _sendStatusCode;
  private readonly IMapperObj _mapperObj;
  private readonly INonceToken _nonceTokenRepository;
  private readonly ILogger<AuthConnectionService> _logger;
  private readonly IClientNotAuthorized _clientNoAuthorized;

  private static ClientInfo? _clientInfoTemp;
  private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;
  private readonly HashSet<string> _hashClientWork = new();
  private readonly HashSet<Guid> _usedNonce = new();

  private static bool PrimaryGetStatus { get; set; }

  public AuthConnectionService(ISend<HttpStatusCode> sendStatusCode,
    IMapperObj mapperObj, INonceToken nonceTokenRepository,
    ILogger<AuthConnectionService> logger, IClientNotAuthorized clientNoAuthorized)
  {
    _sendStatusCode = sendStatusCode;
    _mapperObj = mapperObj;
    _nonceTokenRepository = nonceTokenRepository;
    _logger = logger;
    _clientNoAuthorized = clientNoAuthorized;
    _ = SubscribeEvents();
  }

  private async Task SubscribeEvents()
  {
    _globalEventBus.Subscribe<ClientInfo>((clientInfo) => _clientInfoTemp = clientInfo);

    var tcs = new TaskCompletionSource<ClientHandshakeDto>();

    async Task Handler(ClientHandshakeDto clientHandshakeDto)
    {
      tcs.TrySetResult(clientHandshakeDto);
      await OnReceiveHandshakeDtoAsync(clientHandshakeDto);
    }

    _globalEventBus.Subscribe<ClientHandshakeDto>((handler) => _ = Handler(handler));

    await tcs.Task.WaitAsync(TimeSpan.FromSeconds(5));
  }

  private void GetNonceDb() =>
    _usedNonce.AddRange(_nonceTokenRepository.GetAll().Select(g => g.GuidTokenGlobal));


  public async Task HandleClientAsync(ClientInfo clientInfo, ClientHandshakeRequest clientHandshakeRequest,
    CancellationToken cts = default)
  {
    _clientInfoTemp = clientInfo;
    try
    {
      if (!PrimaryGetStatus)
      {
        GetNonceDb();
        PrimaryGetStatus = true;
      }

      if (_hashClientWork.Any(h => h != clientHandshakeRequest.HashExecHex) ||
          _usedNonce.Any(p => p == clientHandshakeRequest.Nonce))
      {
        await SendStatusOperationAuthAsync(false, clientHandshakeRequest, cts);
        return;
      }

      _usedNonce.Add(clientHandshakeRequest.Nonce);

      var key = Convert.FromHexString(clientHandshakeRequest.HashExecHex);
      var nonceBytes = Encoding.UTF8.GetBytes(clientHandshakeRequest.Nonce.ToString());
      using var expectedMac = new HMACSHA256();
      expectedMac.Key = key;
      var computed = expectedMac.ComputeHash(nonceBytes);

      if (!CryptographicOperations.FixedTimeEquals(computed,
            Convert.FromHexString(clientHandshakeRequest.SignatureHex)))
      {
        await SendStatusOperationAuthAsync(false, clientHandshakeRequest, cts);
        return;
      }

      await SendStatusOperationAuthAsync(true, clientHandshakeRequest, cts);
    }
    catch (Exception e)
    {
      clientInfo.Disconnect();
      _logger.LogError($"An error occurred when authenticating the client. Error: {e.Message}");
      throw new Exception();
    }
  }

  private async Task SendStatusOperationAuthAsync(bool status, ClientHandshakeRequest clientHandshakeRequest,
    CancellationToken cts = default)
  {
    if (status)
    {
      await _sendStatusCode.SendAsync(HttpStatusCode.OK, _clientInfoTemp!, TypeSocketSsl.SslStream, cts);
      _logger.LogInformation($"Signature is valid.");
      return;
    }

    await _sendStatusCode.SendAsync(HttpStatusCode.Unauthorized, _clientInfoTemp!, TypeSocketSsl.SslStream, cts);

    await _clientNoAuthorized.AddAsync(new ClientNotAuthorized(_clientInfoTemp!.SocketWrapper!.RemoteEndPoint!,
      "Client did not pass the initial authentication."), cts);

    _clientInfoTemp.Disconnect();
    _logger.LogCritical($"Signature is not valid." +
                        $" HashExecHex: {clientHandshakeRequest.HashExecHex} Nonce: {clientHandshakeRequest.Nonce}");
  }

  private async Task OnReceiveHandshakeDtoAsync(ClientHandshakeDto clientHandshakeRequestDto)
  {
    var clientHandshakeRequest = _mapperObj.MapToObj(clientHandshakeRequestDto, new ClientHandshakeRequest(
      clientHandshakeRequestDto.HashExecHex!, clientHandshakeRequestDto.SignatureHex!));
    _logger.LogInformation($"Client handshake received: {clientHandshakeRequest}");
    await HandleClientAsync(_clientInfoTemp!, clientHandshakeRequest);
  }
}
