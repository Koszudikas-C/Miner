using LibHandler.EventBus;
using LibRemoteAndClient.Entities.Remote.Client;
using LibSsl.Entities;

namespace UpdateClientService.Connection;

public class ConnectionRemoteInstance
{
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;
    
    private static ConnectionRemoteInstance? _instance; 
    private static readonly object _lock = new();

    public ClientInfo? _clientInfo { get; private set; } 
    
    public static ConnectionRemoteInstance Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new ConnectionRemoteInstance();
                }
                return _instance;
            }
        }
    }
    
    private ConnectionRemoteInstance() { }
    
    public void InitializeSubscribe()
    {
        _globalEventBusClient.Subscribe<ClientInfo>(OnClientInfo);
    }
    
    private void OnClientInfo(ClientInfo clientInfo)
    {
        _clientInfo = clientInfo;
    }
}