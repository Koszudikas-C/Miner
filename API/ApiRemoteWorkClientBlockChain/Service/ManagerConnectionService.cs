using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using ApiRemoteWorkClientBlockChain.Entities.Interface;
using ApiRemoteWorkClientBlockChain.Interface;
using ApiRemoteWorkClientBlockChain.Utils;
using LibCommunicationStateRemote.Entities;
using LibCommunicationStatusRemote.Entities;
using LibCryptographyRemote.Entities;
using LibCryptographyRemote.Interface;
using LibDtoRemote.Dto;
using LibDtoRemote.Dto.ClientMine;
using LibEntitiesRemote.Entities;
using LibEntitiesRemote.Entities.Client;
using LibEntitiesRemote.Entities.Enum;
using LibHandlerRemote.Entities;
using LibManagerFileRemote.Entities.Enum;
using LibManagerFileRemote.Interface;
using LibMapperObjRemote.Interface;
using LibReceiveRemote.Interface;
using LibSendRemote.Interface;
using LibSocketAndSslStreamRemote.Entities;
using LibSocketAndSslStreamRemote.Entities.Enum;
using LibSocketAndSslStreamRemote.Interface;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

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
    private readonly IReceive _receive;
    private readonly IPosAuth _posAuth;

    private readonly GlobalEventBus _globalEventBusRemote = GlobalEventBus.Instance!;

    public ManagerConnectionService(ILogger<ManagerConnectionService> logger, ISocketMiring socketMiring,
        IAuthSsl authSsl, IManagerClient managerClient,
        ISearchFile searchFile, ISend<ConfigCryptographDto> sendConfigCryptographDto,
        ISend<ConfigSaveFileDto> sendConfigSaveFileDto, IClientConnected clientConnected,
        ICryptographFile cryptographFile, IMapperObj mapperObj, IReceive receive, IPosAuth posAuth)
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
        _receive = receive;
        _posAuth = posAuth;

        _globalEventBusRemote.Subscribe<ClientInfo>((handle) => _ = OnClientInfoReceived(handle));
        _globalEventBusRemote.Subscribe<ClientMineDto>(OnClientMineReceived);
    }

    public async Task<ApiResponse<object>> InitializeAsync(ConnectionConfig connectionConfig, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        if (CommunicationStateReceiveAndSend.IsConnected)
            return InstanceApiResponse<object>(HttpStatusCode.Conflict, false,
                $"Remote server already {connectionConfig.Port} in use", null!);
        try
        {
            await _socketMiring.InitializeAsync(connectionConfig.Port, connectionConfig.MaxConnections,
                typeAuthMode, cts).ConfigureAwait(false);

            for (var i = 0; i < 5; i++)
            {
                if (CommunicationStateReceiveAndSend.IsConnecting) break;

                await Task.Delay(1000, cts).ConfigureAwait(false);

                if (i == 4)
                    return InstanceApiResponse<object>(HttpStatusCode.GatewayTimeout, false,
                        "Service initialization failed due to a timeout while attempting to establish" +
                        " communication with the remote server. Please check the connection settings and try again.",
                        null!);
            }

            return InstanceApiResponse<object>(HttpStatusCode.OK, true,
                $"Successful service initialization type {typeAuthMode.ToString()}", null!);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error starting SocketMiringService in mode {typeAuthMode}," +
                             $" Port:{connectionConfig.Port}," +
                             $" MaxConnections:{connectionConfig.MaxConnections}: {e.Message}");
            CommunicationStateReceiveAndSend.SetConnected(false);
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

            await _receive.ReceiveDataAsync(clientInfo, TypeSocketSsl.SslStream, 0);

            await SendConfig(clientInfo).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            ConnectionDisconnected(clientInfo);
            _logger.LogError(
                $"A socketexception on the client side was generated for this reason it will be disconnected: Error: {e.Message}");
            throw new Exception();
        }
    }

    private async Task SendConfig(ClientInfo clientInfo)
    {
        var configCryptograph = await CreateEncryptedConfigAsync(clientInfo);

        await SendConfigSaveFileAsync(configCryptograph, clientInfo).ConfigureAwait(false);

        try
        {
            var dto = _mapperObj.MapToDto(configCryptograph, new ConfigCryptographDto());

            await _posAuth.SendDataAsync(dto, clientInfo.Id);

            await _receive.ReceiveDataAsync(clientInfo, TypeSocketSsl.SslStream);
        }
        catch (SocketException e)
        {
            _logger.LogInformation($"A error in sending the configuration to the client:  Error: {e.Message}");
            ConnectionDisconnected(clientInfo);
            throw new SocketException();
        }
    }

    private async Task<ConfigCryptograph> CreateEncryptedConfigAsync(ClientInfo clientInfo)
    {
        var cryptograph = PosAuth.PreparationFileCryptedConfigVariablePosAuth(clientInfo);

        var dataJson = await _searchFile.SearchFileAsync(TypeFile.ConfigVariable);

        cryptograph.SetData(dataJson);

        var encryptedData = _cryptographFile.SaveFile(cryptograph);

        if (clientInfo.ClientMine!.PlatformSystem == OSPlatform.Windows.ToString())
            cryptograph.FilePath = clientInfo.ClientMine!.PathLocal! + "Resources\\koewa";

        cryptograph.SetDataClear();
        cryptograph.SetDataBytes(encryptedData);

        return cryptograph;
    }

    private async Task SendConfigSaveFileAsync(ConfigCryptograph configCryptographDto,
        ClientInfo clientInfo)
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
            throw new Exception();
        }
    }

    private void ConnectionDisconnected(ClientInfo clientInfo)
    {
        _clientConnected.RemoveClientInfo(clientInfo);
    }

    private void OnClientMineReceived(ClientMineDto obj)
    {
        var clientMine = _mapperObj.Map<ClientMineDto, ClientMine>(obj);
        _logger.LogInformation($"ClientMine received: {clientMine.ClientInfoId}");
        _logger.LogInformation($"Total clients: {_clientConnected.GetClientInfos().Count}");
        _clientConnected.AddClientMine(clientMine);
    }
}
