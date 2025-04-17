using System.Net;
using ApiRemoteWorkClientBlockChain.Entities;
using ApiRemoteWorkClientBlockChain.Interface;
using LibCommunicationStatus.Entities;
using LibHandler.EventBus;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSocket.Interface;

namespace ApiRemoteWorkClientBlockChain.Service;

public class ManagerClientService : IManagerClient
{
    private readonly ILogger<ManagerClientService> _logger;
    private readonly ISocketMiring _socketMiring;
    private readonly IReceive _receive;
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;
    private readonly ClientConnected _clientConnected = ClientConnected.Instance;

    public ManagerClientService(ILogger<ManagerClientService> logger, ISocketMiring socketMiring
        , IReceive receive)
    {
        _logger = logger;
        _socketMiring = socketMiring;
        _receive = receive;
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

    private static ApiResponse<T> InstanceApiResponse<T>(HttpStatusCode statusCode, bool sucess,
        string message, IEnumerable<T> data, List<string>? errors = null) =>
        new ApiResponse<T>(statusCode, sucess, message, data, errors);

    private void OnClientInfoReceived(ClientInfo clientInfo)
    {
        _receive!.ReceiveDataAsync(clientInfo, TypeRemoteClient.Remote,
            1);
        _clientConnected.AddClientInfos(clientInfo);

        if (clientInfo.SslStream!.IsAuthenticated)
        {
            _logger.Log(LogLevel.Information, $"Client connected Auth: {clientInfo.Id}," +
                                              $" {clientInfo.SslStream!.IsAuthenticated}");
            return;
        }

        _logger.Log(LogLevel.Information, $"Client connected: {clientInfo.Id}," +
                                          $" {clientInfo.Socket!.RemoteEndPoint}");
    }
}