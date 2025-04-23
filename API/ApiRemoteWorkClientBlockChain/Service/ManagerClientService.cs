using System.Net;
using ApiRemoteWorkClientBlockChain.Entities.Interface;
using ApiRemoteWorkClientBlockChain.Interface;
using LibCommunicationStatus.Entities;
using LibHandler.EventBus;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSocketAndSslStream.Interface;

namespace ApiRemoteWorkClientBlockChain.Service;

public class ManagerClientService : IManagerClient
{
    private readonly ILogger<ManagerClientService> _logger;
    private readonly ISocketMiring _socketMiring;
    private readonly IReceive _receive;
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;
    private readonly IClientConnected _clientConnected;

    public ManagerClientService(ILogger<ManagerClientService> logger, ISocketMiring socketMiring
        , IReceive receive, IClientConnected clientConnected)
    {
        _logger = logger;
        _socketMiring = socketMiring;
        _receive = receive;
        _clientConnected = clientConnected;
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
        if(clientId == Guid.Empty) return InstanceApiResponse<ClientInfo>(
            HttpStatusCode.NoContent, false, "clientId cannot be empty", null!);
        
        var clientInfo = _clientConnected.GetClientInfo(clientId);
        
        if(clientInfo == null!) return InstanceApiResponse<ClientInfo>(
            HttpStatusCode.NoContent, false, "Client not found", null!);

        return InstanceApiResponse<ClientInfo>(HttpStatusCode.OK, true,
            "Successful", [clientInfo]
        );
    }

    private static ApiResponse<T> InstanceApiResponse<T>(HttpStatusCode statusCode, bool sucess,
        string message, IEnumerable<T> data, List<string>? errors = null) =>
        new ApiResponse<T>(statusCode, sucess, message, data, errors);

    private void OnClientInfoReceived(ClientInfo clientInfo)
    {
        _receive!.ReceiveDataAsync(clientInfo, TypeRemoteClient.Remote,
            1);
        _clientConnected.AddClientInfo(clientInfo);

        if (clientInfo.SslStreamWrapper!.IsAuthenticated)
        {
            _logger.Log(LogLevel.Information, $"Client connected Auth: {clientInfo.Id}," +
                                              $" {clientInfo.SslStreamWrapper!.IsAuthenticated}");
            return;
        }

        _logger.Log(LogLevel.Information, $"Client connected: {clientInfo.Id}," +
                                          $" {clientInfo.SocketWrapper!.RemoteEndPoint}");
    }
}