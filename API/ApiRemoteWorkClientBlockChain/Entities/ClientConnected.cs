using System.Collections.Concurrent;
using LibRemoteAndClient.Entities.Remote.Client;

namespace ApiRemoteWorkClientBlockChain.Entities;

public class ClientConnected
{
    private static readonly Lazy<ClientConnected> _instance = new(() => new ClientConnected());
    public static ClientConnected Instance => _instance.Value;

    private readonly Dictionary<Guid, ClientInfo> _clients = new();

    private ClientConnected() { }

    public List<ClientInfo> AddClientInfos(ClientInfo clientInfo)
    {
        if (_clients.TryGetValue(clientInfo.Id, out var existingClient)) return _clients.Values.ToList();
        
        _clients[clientInfo.Id] = clientInfo;
        return _clients.Values.ToList();
    }

    public List<ClientInfo> RemoveClientInfos(ClientInfo clientInfo)
    {
        _clients.Remove(clientInfo.Id);
        return _clients.Values.ToList();
    }

    public List<ClientInfo> UpdateClientInfo(ClientInfo clientInfo)
    {
        _clients[clientInfo.Id] = clientInfo;
        return _clients.Values.ToList();
    }

    public IReadOnlyCollection<ClientInfo> GetClientInfos() => _clients.Values;
}