using System.Net;
using System.Text.Json;
using ApiRemoteWorkClientBlockChain.Entities.Interface;
using ApiRemoteWorkClientBlockChain.Interface;
using LibCommunicationStatus.Entities;
using LibCryptography.Entities;
using LibCryptography.Interface;
using LibDto.Dto;
using LibHandler.EventBus;
using LibManagerFile.Entities;
using LibManagerFile.Entities.Enum;
using LibManagerFile.Interface;
using LibMapperObj.Interface;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSend.Interface;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Interface;

namespace ApiRemoteWorkClientBlockChain.Service;

public class ManagerClientService : IManagerClient
{
    private readonly ILogger<ManagerClientService> _logger;
    private readonly IReceive _receive;
    private readonly IClientConnected _clientConnected;
    private readonly ISend<ConfigCryptographDto> _send;
    private readonly ISend<ConfigSaveFileDto> _sendConfigSaveFile;
    private readonly ICryptographFile _cryptographFile;
    private readonly ISearchFile _searchFile;
    private readonly IMapperObj _mapperObj;
    
    private readonly string _pathFile = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "koewa.json");

    private readonly string _pathFileDest =
        Path.Combine(Directory.GetCurrentDirectory(), "ResourcesDest", "koewa.json");

    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;

    public ManagerClientService(ILogger<ManagerClientService> logger, IReceive receive,
        IClientConnected clientConnected, ISend<ConfigCryptographDto> send,
        ISend<ConfigSaveFileDto> sendConfigSaveFile, ICryptographFile cryptographFile,
        ISearchFile searchFile, IMapperObj mapperObj)
    {
        _logger = logger;
        _receive = receive;
        _clientConnected = clientConnected;
        _send = send;
        _sendConfigSaveFile = sendConfigSaveFile;
        _cryptographFile = cryptographFile;
        _searchFile = searchFile;
        _mapperObj = mapperObj;
        _globalEventBusRemote.Subscribe<ClientInfo>(OnClientInfoReceived);
    }

    public ApiResponse<ClientInfo> GetAllClientInfo(int page, int pageSize)
    {
        try
        {
            if (pageSize > 50) pageSize = 50;

            var listClientIfo = _clientConnected.GetClientInfos().Skip((page - 1) * pageSize).Take(pageSize).ToList();

            if (listClientIfo.Count != 0)
            {
                return InstanceApiResponse(HttpStatusCode.OK, true,
                    "Successful", listClientIfo);
            }

            return InstanceApiResponse<ClientInfo>(HttpStatusCode.NoContent, false,
                "No existing clients", null!);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public ApiResponse<ClientInfo> GetClientInfo(Guid clientId)
    {
        if (clientId == Guid.Empty)
            return InstanceApiResponse<ClientInfo>(
                HttpStatusCode.NoContent, false, "clientId cannot be empty", null!);

        var clientInfo = _clientConnected.GetClientInfo(clientId);

        if (clientInfo == null!)
            return InstanceApiResponse<ClientInfo>(
                HttpStatusCode.NoContent, false, "Client not found", null!);

        return InstanceApiResponse<ClientInfo>(HttpStatusCode.OK, true,
            "Successful", [clientInfo]
        );
    }

    public async Task<ApiResponse<object>> SendFileConfigVariableAsync(
        ConfigCryptographDto configCryptographDto,
        Guid clientId, CancellationToken cts = default)
    {
        try
        {
            var clientInfo = _clientConnected.GetClientInfo(clientId);

            if (!clientInfo.SslStreamWrapper!.IsAuthenticated)
                return InstanceApiResponse<object>(HttpStatusCode.Unauthorized,
                    false, $"Unauthorized client: {clientInfo.Id}", null!);

            await _send.SendAsync(configCryptographDto, clientInfo, TypeSocketSsl.SslStream, cts);

            return InstanceApiResponse<object>(HttpStatusCode.OK, true,
                "Successful send", null!);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _logger.LogError($"Error in SendConfigVariableOneAsync: {e.Message}");

            return InstanceApiResponse<object>(HttpStatusCode.InternalServerError,
                false, "Error sending the encryption configuration to the client", null!);
        }
    }

    private static ApiResponse<T> InstanceApiResponse<T>(HttpStatusCode statusCode, bool sucess,
        string message, IEnumerable<T> data, List<string>? errors = null) =>
        new ApiResponse<T>(statusCode, sucess, message, data, errors);

    private void OnClientInfoReceived(ClientInfo clientInfo)
    {
        try
        {
            _receive!.ReceiveDataAsync(clientInfo, TypeSocketSsl.Socket,
                1);
            _clientConnected.AddClientInfo(clientInfo);

            if (clientInfo.SslStreamWrapper!.IsAuthenticated)
            {
                _logger.Log(LogLevel.Information, $"Client connected Auth: {clientInfo.Id}," +
                                                  $" {clientInfo.SslStreamWrapper!.IsAuthenticated}");
            }
            else
            {
                _logger.Log(LogLevel.Information, $"Client connected: {clientInfo.Id}," +
                                                  $" {clientInfo.SocketWrapper!.RemoteEndPoint}");
            }

            var configCrConfigVariable = PreparationFileCryptedConfigVariable();
            
            SendConfigSaveFile(clientInfo, configCrConfigVariable);

            var configCryptographDto = _mapperObj.MapToDto(configCrConfigVariable, new ConfigCryptographDto());

            _ = SendFileConfigVariableAsync(configCryptographDto, clientInfo.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _logger.LogError($"Error in OnClientInfoReceived: {e.Message}");
        }
    }

    private void SendConfigSaveFile(ClientInfo clientInfo,
        ConfigCryptograph configCryptographDto)
    {
        var configSaveFileDto = new ConfigSaveFileDto()
        {
            FileName = "koewa",
            DataBytes = configCryptographDto.GetDataBytes()
        };
        
        configSaveFileDto.ExtensionFile = TypeExtensionFile.Json;
        
        _sendConfigSaveFile.SendAsync(configSaveFileDto, clientInfo, TypeSocketSsl.SslStream);
    }

    private ConfigCryptograph PreparationFileCryptedConfigVariable()
    {
        var configCrConfigVariable = new ConfigCryptograph(_pathFileDest)
        {
            Key = "51FB3317651C452185A3ADA203F3FF9C",
            HmacKey = "5D7612C3E2E44698AEDF4E8BBA2790EA"
        };

        var data = _searchFile.SearchFile(TypeFile.ConfigVariable);

        if (File.Exists(_pathFileDest))
            File.Delete(_pathFileDest);

        File.Copy(_pathFile, _pathFileDest);

        configCrConfigVariable.SetData(data!);

        var dataEncrypted = _cryptographFile.SaveFile(configCrConfigVariable!);
        
        configCrConfigVariable.SetDataBytes(dataEncrypted);

        return configCrConfigVariable;
    }
}