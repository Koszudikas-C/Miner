using System.Net;
using LibCommunicationStateClient.Entities;
using LibCryptographyClient.Entities;
using LibCryptographyClient.Interface;
using LibDtoClient.Dto;
using LibDtoClient.Dto.ClientMine;
using LibEntitiesClient.Entities;
using LibEntitiesClient.Entities.Enum;
using LibHandlerClient.Entities;
using LibManagerFileClient.Entities;
using LibManagerFileClient.Interface;
using LibMapperObjClient.Interface;
using LibReceiveClient.Interface;
using LibSendClient.Interface;
using WorkClientBlockChain.Utils;
using WorkClientBlockChain.Utils.Interface;

namespace WorkClientBlockChain.Service;

public class PosAuthService(
  IReceive receive,
  ISend<HttpStatusCode> send,
  ILogger<PosAuthService> logger,
  ISaveFile saveFile,
  ICryptographFile cryptographFile,
  IMapperObj mapperObj,
  ISend<ClientMineDto> sendClientMine) : IPosAuth
{
  private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;
  private ConfigSaveFile? _configSaveFile;

  public async Task SendClientMine(ClientInfo clientInfo)
  {
    try
    {
      var clientMineDto = PosAuthUtil.GetClientDtoDefault();
      await sendClientMine.SendAsync(clientMineDto, clientInfo, TypeSocketSsl.SslStream);
    }
    catch (Exception e)
    {
      logger.LogError($"Error sending mining data to the customer. Discarding connection. Error: {e.Message}");
      DisconnectClient();
      throw new Exception();
    }
  }

  public async Task ReceiveDataCrypt(ClientInfo clientInfo)
  {
    if (!clientInfo.SslStreamWrapper?.InnerSslStream!.IsAuthenticated ?? false)
    {
      CommunicationStateReceiveAndSend.SetConnected(false);
      DisconnectClient();
      return;
    }

    try
    {
      _globalEventBus.Subscribe<ConfigSaveFileDto>(Handler);
      _globalEventBus.Subscribe<ConfigCryptographDto>(OnClientInfoReceived);

      logger.LogInformation("Awaiting receiving the configuration file...");
      await receive.ReceiveDataAsync(clientInfo, TypeSocketSsl.SslStream, 2);
    }
    catch (Exception ex)
    {
      logger.LogError($"Error receiving encrypted data. Discarding connection. Error:{ex.Message}");
      DisconnectClient();
      throw new Exception();
    }
  }

  private void Handler(ConfigSaveFileDto configSaveFileDto)
  {
    try
    {
      configSaveFileDto.PathFile ??= "PathDefault";
      _configSaveFile = mapperObj.MapToObj(configSaveFileDto,
        new ConfigSaveFile(configSaveFileDto.FileName!, configSaveFileDto.PathFile!));

      saveFile.SaveFileWriteBytes(_configSaveFile!);
      logger.LogInformation("Configuration file received.");
    }
    catch (Exception e)
    {
      logger.LogError($"Error processing configSaveFileDto. Error: {e.Message}");
      DisconnectClient();
      throw new Exception();
    }
  }

  private void OnClientInfoReceived(ConfigCryptographDto configCryptographDto)
  {
    try
    {
      configCryptographDto.FilePath ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources",
        _configSaveFile?.FileName ?? "koewa.json");

      var configCryptograph = mapperObj.MapToObj(configCryptographDto,
        new ConfigCryptograph(configCryptographDto.FilePath!));

      var result = cryptographFile.LoadFile(configCryptograph);

      logger.LogInformation($"Successful configcryptograph received and charged: {result}");

      _globalEventBus.Unsubscribe<ConfigCryptographDto>(OnClientInfoReceived);
    }
    catch (Exception e)
    {
      logger.LogError($"Error processing ConfigCryptographDto. Discarding connection. Error: {e.Message}");
      DisconnectClient();
      throw new Exception();
    }
  }

  private static void DisconnectClient()
  {
    CommunicationStateReceiveAndSend.SetConnected(false);
  }
}
