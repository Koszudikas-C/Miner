using LibHandler.EventBus;
using LibHandler.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using WorkClientBlockChain.Connection.Interface;

namespace WorkClientBlockChain.Connection;

public class ClientContext : IClientContext
{
    private static readonly object _lock = new();
    private static ClientInfo? _clientInfo;
    
    private static readonly GlobalEventBusClient GlobalEventBusClient = GlobalEventBusClient.Instance!;
    
    private static readonly Lazy<ClientContext> _instance = new(() => new ClientContext());

    public static IClientContext Instance => _instance.Value;

    private ClientContext()
    {
        GlobalEventBusClient.Subscribe<ClientInfo>(OnClientInfoReceived);
    }

    private void OnClientInfoReceived(ClientInfo? obj)
    {
        SetClientInfo(obj);
    }

    public ClientInfo? GetClientInfo()
    {
        lock (_lock)
        {
            return _clientInfo;
        }
    }

    public void SetClientInfo(ClientInfo? info)
    {
        lock (_lock)
        {
            _clientInfo = info;
        }
    }

    public void Reset()
    {
        SetClientInfo(null);
    }
}