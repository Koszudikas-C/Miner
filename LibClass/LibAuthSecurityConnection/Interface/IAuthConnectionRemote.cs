using LibRemoteAndClient.Entities.Client;
using LibRemoteAndClient.Entities.Remote;
using LibRemoteAndClient.Entities.Remote.Client;

namespace LibAuthSecurityConnection.Interface;

public interface IAuthConnectionRemote
{
    Task HandleClientAsync(ClientInfo clientInfo, ClientHandshakeRequest clientHandshake,
        CancellationToken cts =default);
}