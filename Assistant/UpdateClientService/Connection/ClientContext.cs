using LibHandler.EventBus;
using LibRemoteAndClient.Entities.Remote.Client;

namespace UpdateClientService.Connection;

public sealed class ClientContext
{
    private static readonly object? _lock = new();
    private static ClientInfo? _clientInfo;
    private static readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;
    
    private static readonly Lazy<ClientContext> _instance = new(() => new ClientContext());

    public static ClientContext Instance => _instance.Value;

    public ClientInfo? ClientInfo()
    {
        return _clientInfo ?? null;
    }

    private ClientContext()
    {
        _globalEventBusClient.Subscribe<ClientInfo>(OnClientInfoReceived);
    }

    private static void OnClientInfoReceived(ClientInfo? obj)
    {
        lock (_lock!)
        {
            _clientInfo = obj;
        }
    }
}