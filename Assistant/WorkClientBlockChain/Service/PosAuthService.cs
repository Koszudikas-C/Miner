using System.Net;
using LibCommunicationStatus;
using LibCryptography.Entities;
using LibCryptography.Interface;
using LibDto.Dto;
using LibDto.Dto.ClientMine;
using LibHandler.EventBus;
using LibManagerFile.Entities;
using LibManagerFile.Interface;
using LibMapperObj.Interface;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSend.Interface;
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
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;
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
            CommunicationStatus.SetConnected(false);
            DisconnectClient();
            return;
        }

        try
        {
            _globalEventBusClient.Subscribe<ConfigSaveFileDto>(Handler);
            _globalEventBusClient.Subscribe<ConfigCryptographDto>(OnClientInfoReceived);

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

            _globalEventBusClient.Unsubscribe<ConfigCryptographDto>(OnClientInfoReceived);
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
        CommunicationStatus.SetConnected( false);
    }
}