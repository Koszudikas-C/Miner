using LibEntitiesClient.Entities;

namespace LibSocketAndSslStreamClient.Interface;

public interface IAuthSsl
{
    Task AuthenticateAsync(ObjSocketSslStream objSocketSslStream,
        CancellationToken cts = default);

    void Reconnect(Guid clientId);
}
