using System.Net;
using System.Security.Cryptography;
using ApiRemoteWorkClientBlockChain.Entities.Interface;
using ApiRemoteWorkClientBlockChain.Interface;
using LibCommunicationStatus.Entities;
using LibCryptography.Entities;
using LibCryptography.Interface;
using LibDto.Dto;
using LibHandler.EventBus;
using LibManagerFile.Entities.Enum;
using LibManagerFile.Interface;
using LibMapperObj.Interface;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSend.Interface;

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

    private static ApiResponse<T> InstanceApiResponse<T>(HttpStatusCode statusCode, bool success,
        string message, IEnumerable<T> data, List<string>? errors = null) =>
        new ApiResponse<T>(statusCode, success, message, data, errors);
}
