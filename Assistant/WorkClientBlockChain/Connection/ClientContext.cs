using LibHandler.EventBus;
using LibHandler.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.Utils.Interface;

namespace WorkClientBlockChain.Connection;

public class ClientContext : IClientContext
{
    private static readonly object _lock = new();
    private static ClientInfo? _clientInfo;
    
    private readonly IPosAuth _posAuth; 
    
    private static readonly GlobalEventBusClient GlobalEventBusClient = GlobalEventBusClient.Instance!;
    
    public ClientContext(IPosAuth posAuth)
    {
        _posAuth = posAuth;
        GlobalEventBusClient.Subscribe<ClientInfo>(OnClientInfoReceived);
    }

    private void OnClientInfoReceived(ClientInfo? obj)
    {
        SetClientInfo(obj);
        _posAuth.ReceiveDataCrypt(obj!);
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