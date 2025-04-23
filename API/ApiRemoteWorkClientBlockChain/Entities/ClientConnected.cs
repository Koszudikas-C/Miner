using System.Collections.Concurrent;
using ApiRemoteWorkClientBlockChain.Entities.Interface;
using LibRemoteAndClient.Entities.Remote.Client;

namespace ApiRemoteWorkClientBlockChain.Entities;

public class ClientConnected : IClientConnected
{
    private static Lazy<ClientConnected> _instance = new(() => new ClientConnected());

    public static ClientConnected Instance => _instance.Value;

    private readonly Dictionary<Guid, ClientInfo> _clients = new();

    private Guid ClientInfoLastRequirementId { get; set; }

    private ClientConnected()
    {
    }

    public List<ClientInfo> AddClientInfo(ClientInfo clientInfo)
    {
        if (_clients.TryGetValue(clientInfo.Id, out var existingClient)) return _clients.Values.ToList();

        _clients[clientInfo.Id] = clientInfo;
        return _clients.Values.ToList();
    }

    public List<ClientInfo> RemoveClientInfo(ClientInfo clientInfo)
    {
        _clients.Remove(clientInfo.Id);
        return _clients.Values.ToList();
    }

    public List<ClientInfo> UpdateClientInfo(ClientInfo clientInfo)
    {
        _clients[clientInfo.Id] = clientInfo;
        return _clients.Values.ToList();
    }

    public ClientInfo GetClientInfoLastRequirement() => _clients.GetValueOrDefault(ClientInfoLastRequirementId)!;

    public ClientInfo GetClientInfo(Guid clientId, bool removeLastRequirement)
    {
        ClientInfoLastRequirementId = removeLastRequirement ? Guid.Empty : clientId;

        return _clients.GetValueOrDefault(clientId)!;
    }

    public List<ClientInfo> GetClientInfos() => _clients.Values.ToList();

    public void Clear()
    {
        _clients.Clear();
    }
}