using LibRemoteAndClient.Entities.Remote;
using LibRemoteAndClient.Entities.Remote.Client;

namespace LibClassAuthSecurityConnection.Interface;

public interface IAuthConnectionClient
{
    Task HandleServerAsync(ClientInfo clientInfo, Guid nonceToken,
        CancellationToken cts = default);
}