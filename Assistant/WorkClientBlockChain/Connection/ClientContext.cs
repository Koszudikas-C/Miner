using LibHandler.EventBus;
using LibHandler.Interface;
using LibRemoteAndClient.Entities.Remote.Client;

namespace WorkClientBlockChain.Connection;

public class ClientContext
{
    private static readonly object _lock = new();
    private static ClientInfo? _clientInfo;
    
    private static readonly GlobalEventBusClient GlobalEventBusClient = GlobalEventBusClient.Instance!;
    
    private static readonly Lazy<ClientContext> _instance = new(() => new ClientContext());

    public static ClientContext Instance => _instance.Value;

    private ClientContext()
    {
        GlobalEventBusClient.Subscribe<ClientInfo>(OnClientInfoReceived);
    }

    private static void OnClientInfoReceived(ClientInfo? obj)
    {
        lock (_lock)
        {
            _clientInfo = obj;
        }
    }

    public static ClientInfo? GetClientInfo()
    {
        return _clientInfo;
    }
    
    public static void Reset()
    {
        lock (_lock)
        {
            _clientInfo = null;
        }
    }
}