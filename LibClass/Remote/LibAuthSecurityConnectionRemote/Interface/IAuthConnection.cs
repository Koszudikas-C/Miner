using LibEntitiesRemote.Entities;

namespace LibAuthSecurityConnectionRemote.Interface;

public interface IAuthConnection
{
    Task HandleClientAsync(ClientInfo clientInfo, ClientHandshakeRequest clientHandshake,
        CancellationToken cts =default);
}
