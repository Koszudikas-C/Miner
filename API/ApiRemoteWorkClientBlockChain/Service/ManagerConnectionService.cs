using System.Net;
using ApiRemoteWorkClientBlockChain.Entities.Interface;
using LibRemoteAndClient.Enum;
using ApiRemoteWorkClientBlockChain.Interface;
using ApiRemoteWorkClientBlockChain.Util;
using LibCommunicationStatus;
using LibCommunicationStatus.Entities;
using LibCryptography.Entities;
using LibCryptography.Interface;
using LibDto.Dto;
using LibHandler.EventBus;
using LibManagerFile.Entities.Enum;
using LibManagerFile.Interface;
using LibMapperObj.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibSend.Interface;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Entities.Enum;
using LibSocketAndSslStream.Interface;

namespace ApiRemoteWorkClientBlockChain.Service;

public class ManagerConnectionService : IManagerConnection
{
    private readonly ILogger<ManagerConnectionService> _logger;
    private readonly ISocketMiring _socketMiring;
    private readonly IAuthSsl _authSsl;
    private readonly IManagerClient _managerClient;
    private readonly ISearchFile _searchFile;
    private readonly ISend<ConfigCryptographDto> _sendConfigCryptographDto;
    private readonly ISend<ConfigSaveFileDto> _sendConfigSaveFileDto;
    private readonly IClientConnected _clientConnected;
    private readonly ICryptographFile _cryptographFile;
    private readonly IMapperObj _mapperObj;
    
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;
    public ManagerConnectionService(ILogger<ManagerConnectionService> logger, ISocketMiring socketMiring,
        IAuthSsl authSsl, IManagerClient managerClient, 
        ISearchFile searchFile, ISend<ConfigCryptographDto> sendConfigCryptographDto, 
        ISend<ConfigSaveFileDto> sendConfigSaveFileDto, IClientConnected clientConnected,
        ICryptographFile cryptographFile, IMapperObj mapperObj)
    {
        _logger = logger;
        _socketMiring = socketMiring;
        _authSsl = authSsl;
        _managerClient = managerClient;
        _searchFile = searchFile;
        _sendConfigCryptographDto = sendConfigCryptographDto;
        _sendConfigSaveFileDto = sendConfigSaveFileDto;
        _clientConnected = clientConnected;
        _cryptographFile = cryptographFile;
        _mapperObj = mapperObj;
        
        _globalEventBusRemote.Subscribe<ClientInfo>(async void (clientInfo) => 
            await OnClientInfoReceived(clientInfo));
    }

    public async Task<ApiResponse<object>> InitializeAsync(ConnectionConfig connectionConfig, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        if(CommunicationStatus.IsConnected)
            return InstanceApiResponse<object>(HttpStatusCode.Conflict, false,
                $"Remote server already {connectionConfig.Port} in use", null!);
        try
        {
            await _socketMiring.InitializeAsync(connectionConfig.Port, connectionConfig.MaxConnections,
                typeAuthMode, cts).ConfigureAwait(false);

            for (var i = 0; i < 5; i++)
            {
                if (CommunicationStatus.IsConnecting) break;
                
                await Task.Delay(1000, cts).ConfigureAwait(false);

                if (i == 4)
                    return InstanceApiResponse<object>(HttpStatusCode.GatewayTimeout, false, 
                        "Service initialization failed due to a timeout while attempting to establish" +
                        " communication with the remote server. Please check the connection settings and try again.", null!);
            }
            
            return InstanceApiResponse<object>(HttpStatusCode.OK, true, 
                $"Successful service initialization type {typeAuthMode.ToString()}", null!);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _logger.LogError($"Error starting SocketMiringService in mode {typeAuthMode}," +
                             $" Port:{connectionConfig.Port}," +
                             $" MaxConnections:{connectionConfig.MaxConnections}: {e.Message}");
            CommunicationStatus.SetConnected(false);
            return InstanceApiResponse<object>(HttpStatusCode.InternalServerError, false,
                "Service initialization failed. Verify the settings and try again.", null!);
        }
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

            await _sendConfigCryptographDto.SendAsync(configCryptographDto,
                clientInfo, TypeSocketSsl.SslStream, cts);

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
    
    private static ApiResponse<T> InstanceApiResponse<T>(HttpStatusCode statusCode, bool success, 
        string message, IEnumerable<T> data, List<string>? errors = null) 
        => new ApiResponse<T>(statusCode, success, message, data, errors);
    
    
    private async Task OnClientInfoReceived(ClientInfo clientInfo)
    {
        try
        {
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
            
            await SendConfig(clientInfo);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _logger.LogError($"Error in OnClientInfoReceived: {e.Message}");
            throw new Exception(e.Message);
        }
    }

    private async Task SendConfig(ClientInfo clientInfo)
    {
        var configCryptographConfigVariable = PosAuth.PreparationFileCryptedConfigVariablePosAuth();

        var dataJson = await _searchFile.SearchFileAsync(TypeFile.ConfigVariable);
        
        configCryptographConfigVariable.SetData(dataJson);
        
        var data = _cryptographFile.SaveFile(configCryptographConfigVariable);
        
        configCryptographConfigVariable.SetDataClear();
        
        configCryptographConfigVariable.SetDataBytes(data);

        await SendConfigSaveFileAsync(configCryptographConfigVariable, clientInfo).ConfigureAwait(false);
        
        var configCryptographDto = _mapperObj.MapToDto(configCryptographConfigVariable, new ConfigCryptographDto());
            
        await SendFileConfigVariableAsync(configCryptographDto, clientInfo.Id);
    }

    private async Task SendConfigSaveFileAsync(ConfigCryptograph configCryptographDto, ClientInfo clientInfo)
    {
        try
        {
            var configSaveFileDto = PosAuth.ConfigSaveFileDtoPosAuth(clientInfo, configCryptographDto);
            
            await _sendConfigSaveFileDto.SendAsync(configSaveFileDto, clientInfo, TypeSocketSsl.SslStream);
        }
        catch (Exception e)
        {
            _logger.LogError($"An error occurred when sending the file configuration files: {e.Message}");
            ConnectionDisconnected(clientInfo);
            throw new Exception(e.Message);
        }
    }

    private void ConnectionDisconnected(ClientInfo clientInfo)
    {
        _clientConnected.RemoveClientInfo(clientInfo);
    }
}