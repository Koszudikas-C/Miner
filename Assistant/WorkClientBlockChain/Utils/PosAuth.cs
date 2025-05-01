using System.Net;
using LibCommunicationStatus;
using LibCryptography.Entities;
using LibCryptography.Interface;
using LibDto.Dto;
using LibHandler.EventBus;
using LibManagerFile.Entities;
using LibManagerFile.Interface;
using LibMapperObj.Interface;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSend.Interface;
using LibSocketAndSslStream.Entities;
using WorkClientBlockChain.Utils.Interface;

namespace WorkClientBlockChain.Utils;

public class PosAuth(IReceive receive, ISend<HttpStatusCode> send,
    ILogger<PosAuth> logger, ISaveFile saveFile, ICryptographFile cryptographFile,
    IMapperObj mapperObj) : IPosAuth
{
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;
    private ConfigSaveFile? _configSaveFile;
    
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
    
   public  void Handler(ConfigSaveFileDto configSaveFileDto)
    {
        _configSaveFile = mapperObj.MapToObj(configSaveFileDto, new ConfigSaveFile());
        
        Console.WriteLine("ConfigSaveFile received");
    }

    private void OnClientInfoReceived(ConfigCryptographDto configCryptographDto)
    {
        saveFile.SaveFileByteWrite(_configSaveFile!);
        
        var configCryptograph = mapperObj.MapToObj(configCryptographDto, new ConfigCryptograph(configCryptographDto.FilePath!));
        
       var result = cryptographFile.LoadFile(configCryptograph);
        
       logger.LogInformation($"ConfigCryptograph receive: {result}");
        _globalEventBusClient.Unsubscribe<ConfigCryptographDto>(OnClientInfoReceived);
    }
}