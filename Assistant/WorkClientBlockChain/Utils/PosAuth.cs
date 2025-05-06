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
using WorkClientBlockChain.Utils.Interface;

namespace WorkClientBlockChain.Utils;

public class PosAuth(IReceive receive, ISend<HttpStatusCode> send,
    ILogger<PosAuth> logger, ISaveFile saveFile, ICryptographFile cryptographFile,
    IMapperObj mapperObj, ISend<ClientMineDto> sendClientMine) : IPosAuth
{
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;
    private ConfigSaveFile? _configSaveFile;

    public async Task SendClientMine(ClientInfo clientInfo)
    {
        try
        {
            var clientMine = new LibRemoteAndClient.Entities.Client.ClientMine();
            
            var clientMineDto = mapperObj.Map<LibRemoteAndClient.Entities.Client.ClientMine, ClientMineDto>(clientMine);
            clientMineDto.ClientInfoId = clientInfo.Id;
            
           await sendClientMine.SendAsync(clientMineDto, clientInfo, TypeSocketSsl.SslStream);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task ReceiveDataCrypt(ClientInfo clientInfo)
    {
        if (!clientInfo.SslStreamWrapper!.InnerSslStream!.IsAuthenticated)
        {
            CommunicationStatus.SetConnected(false);
            return;
        }
        
        _globalEventBusClient.Subscribe<ConfigSaveFileDto>(Handler);
        _globalEventBusClient.Subscribe<ConfigCryptographDto>(OnClientInfoReceived);
        
        logger.LogInformation("Awaiting receiving the configuration file");

        await receive.ReceiveDataAsync(clientInfo, TypeSocketSsl.SslStream, 1);
    }

    private void Handler(ConfigSaveFileDto configSaveFileDto)
   {
       configSaveFileDto.PathFile ??= "PathDefault";
        
        _configSaveFile = mapperObj.MapToObj(configSaveFileDto, 
            new ConfigSaveFile(configSaveFileDto.FileName!, configSaveFileDto.PathFile!));
       
        saveFile.SaveFileWriteBytes(_configSaveFile!);
        Console.WriteLine("ConfigSaveFile received");
    }

    private void OnClientInfoReceived(ConfigCryptographDto configCryptographDto)
    {
        try
        {
            var configCryptograph = mapperObj.MapToObj(configCryptographDto,
                new ConfigCryptograph(configCryptographDto.FilePath!));

            var result = cryptographFile.LoadFile(configCryptograph);
        
            logger.LogInformation($"ConfigCryptograph receive: {result}");
            _globalEventBusClient.Unsubscribe<ConfigCryptographDto>(OnClientInfoReceived);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            configCryptographDto.FilePath ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources",
                _configSaveFile!.FileName);
            
            logger.LogError($"Error in OnClientInfoReceived: {e.Message}");
            OnClientInfoReceived(configCryptographDto);
        }
    }
}