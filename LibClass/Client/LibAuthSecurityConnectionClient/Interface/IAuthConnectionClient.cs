using LibEntitiesClient.Entities;

namespace LibAuthSecurityConnectionClient.Interface;

public interface IAuthConnectionClient
{
    Task HandleServerAsync(ClientInfo clientInfo, Guid nonceToken,
        CancellationToken cts = default);
}
