using System.Net;
using System.Security.Cryptography;
using System.Text;
using LibAuthSecurityConnectionClient.Interface;
using LibCommunicationStateClient.Entities;
using LibDtoClient.Dto;
using LibEntitiesClient.Entities;
using LibEntitiesClient.Entities.Enum;
using LibHandlerClient.Entities;
using LibReceiveClient.Interface;
using LibSendClient.Interface;

namespace WorkClientBlockChain.Service.LibClass;

public class AuthConnectionClientService : IAuthConnectionClient
{
  private readonly IReceive _receive;
  private readonly ISend<HttpStatusCode> _sendStatusCode;
  private readonly ISend<ClientHandshakeDto> _sendClientHandshakeDto;
  private readonly ILogger<AuthConnectionClientService> _logger;
  private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;

  public AuthConnectionClientService(IReceive receive, ISend<HttpStatusCode> sendStatusCode,
    ILogger<AuthConnectionClientService> logger, ISend<ClientHandshakeDto> sendClientHandshakeDto)

  {
    _receive = receive;
    _sendStatusCode = sendStatusCode;
    _logger = logger;
    _sendClientHandshakeDto = sendClientHandshakeDto;
    SubscribeEvents();
  }

  private void SubscribeEvents()
  {
    _globalEventBus.Subscribe<ClientInfo, GuidTokenAuthDto>((clientInfo, nonceToken) =>
      _ = OnReceiveNonceTokenTask(clientInfo, nonceToken));
  }

  public async Task HandleServerAsync(ClientInfo clientInfo, Guid nonceToken,
    CancellationToken cts = default)
  {
    if (nonceToken == Guid.Empty)
    {
      await SendStatusOperationAuthAsync(false, clientInfo, cts);
      return;
    }

    try
    {
      var hashExec = ComputeSha2560FCurrentExecutable();
      using var mac = new HMACSHA256();
      mac.Key = hashExec;
      var computeHash = mac.ComputeHash(Encoding.UTF8.GetBytes(nonceToken.ToString()));

      var clientHandshakeDto = new ClientHandshakeDto()
      {
        HashExecHex = Convert.ToHexString(hashExec),
        Nonce = nonceToken,
        SignatureHex = Convert.ToHexString(computeHash)
      };

      await _sendClientHandshakeDto.SendAsync(clientHandshakeDto, clientInfo,
        TypeSocketSsl.SslStream, cts);

      _logger.LogInformation($"Sending client handshake to server.");

      await ReceiveStatusOperationAuthAsync(clientInfo, cts);
    }
    catch (Exception e)
    {
      _logger.LogError($"Error authenticating with the server. Error: {e.Message}");
      throw new Exception();
    }
  }

  private static byte[] ComputeSha2560FCurrentExecutable()
  {
    var exePath = Environment.ProcessPath;
    return SHA256.HashData(File.ReadAllBytes(exePath!));
  }


  private async Task SendStatusOperationAuthAsync(bool statusOperationAuth, ClientInfo clientInfo,
    CancellationToken cts = default)
  {
    if (statusOperationAuth)
    {
      _logger.LogInformation($"The client has been successfully authenticated.");
      await _sendStatusCode.SendAsync(HttpStatusCode.OK, clientInfo,
        TypeSocketSsl.SslStream, cts);
      return;
    }

    _logger.LogInformation($"The client has not been authenticated.");
    await _sendStatusCode.SendAsync(HttpStatusCode.Unauthorized, clientInfo,
      TypeSocketSsl.SslStream, cts);
  }

  private async Task ReceiveStatusOperationAuthAsync(ClientInfo clientInfo, CancellationToken cts = default)
  {
    try
    {
      var task = new TaskCompletionSource<HttpStatusCode>();

      void Handler(HttpStatusCode httpStatusCode)
      {
        task.TrySetResult(httpStatusCode);

        StatusOperationAuth(httpStatusCode, clientInfo);
        SubscribeEvents();
      }

      _globalEventBus.Subscribe<HttpStatusCode>(Handler);
      await _receive.ReceiveDataAsync(clientInfo, TypeSocketSsl.SslStream, 0, cts);

      await task.Task.WaitAsync(TimeSpan.FromSeconds(5), cts);
    }
    catch (Exception e)
    {
      _logger.LogError($"Error receiving the status of the server authentication operation. Error: {e.Message}");
      throw new Exception();
    }
  }

  private void StatusOperationAuth(HttpStatusCode httpStatusCode, ClientInfo clientInfo)
  {
    switch (httpStatusCode)
    {
      case HttpStatusCode.OK:
        _logger.LogInformation($"The client has been successfully authenticated.");
        CommunicationStateReceiveAndSend.SetSending(false);
        _globalEventBus.Publish(clientInfo);
        UnsubscribeEvents();
        break;
      case HttpStatusCode.Unauthorized:
        _logger.LogInformation($"The client has not been authenticated response from the server.");
        DisconnectServer();
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(httpStatusCode), httpStatusCode, null);
    }
  }

  private static void DisconnectServer()
  {
    CommunicationStateReceiveAndSend.SetConnected(false);
    CommunicationStateReceiveAndSend.SetSending(false);
  }

  private void UnsubscribeEvents()
  {
    _globalEventBus.Subscribe<ClientInfo, GuidTokenAuthDto>((clientInfo, nonceToken) =>
      _ = OnReceiveNonceTokenTask(clientInfo, nonceToken));
  }

  private async Task OnReceiveNonceTokenTask(ClientInfo clientInfo, GuidTokenAuthDto guidTokenAuth)
  {
    _logger.LogInformation($"Nonce token received from the server. Nonce: {guidTokenAuth.GuidTokenGlobal}");
    await HandleServerAsync(clientInfo, guidTokenAuth.GuidTokenGlobal);
  }
}
