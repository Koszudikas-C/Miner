using ApiRemoteWorkClientBlockChain.Entities;
using LibHandler.EventBus;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSocket.Entities;
using LibSocket.Interface;
using ApiRemoteWorkClientBlockChain.Interface;
using LibSocket.Entities.Enum;
using LibSsl.Interface;

namespace ApiRemoteWorkClientBlockChain.Service;

public class ManagerConnectionService : IManagerConnection
{
    private readonly ILogger<ManagerConnectionService> _logger;
    private readonly ISocketMiring _socketMiring;
    private readonly IAuthSsl _authSsl;
    private readonly IReceive _receive;
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;
    private readonly ClientConnected _clientConnected = ClientConnected.Instance;
    
    public ManagerConnectionService(ILogger<ManagerConnectionService> logger, ISocketMiring socketMiring
        ,IAuthSsl authSsl, IReceive receive)
    {
        _logger = logger;
        _socketMiring = socketMiring;
        _authSsl = authSsl;
        _receive = receive;
        _globalEventBusRemote.Subscribe<ClientInfo>(OnClientInfoReceived);
    }
    
    public async Task InitializeAsync(ConnectionConfig connectionConfig,
        CancellationToken cts = default)
    {
        try
        {
            await _socketMiring!.InitializeAsync(connectionConfig.Port, connectionConfig.MaxConnections, 
                TypeRemoteClient.Remote, TypeAuthMode.AllowAnonymous, cts);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public IReadOnlyCollection<ClientInfo> GetClientLast() => _clientConnected.GetClientInfos();
    
    private void OnClientInfoReceived(ClientInfo clientInfo)
    {
        _receive!.ReceiveDataAsync(clientInfo,TypeRemoteClient.Remote,
            1);
        _clientConnected.AddClientInfos(clientInfo);
        
        _logger.Log(LogLevel.Information, $"Client connected: {clientInfo.Id}," +
                                          $" {clientInfo.Socket!.RemoteEndPoint}");
        
    }
}