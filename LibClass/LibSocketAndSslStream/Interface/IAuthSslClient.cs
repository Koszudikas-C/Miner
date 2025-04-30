using LibSocketAndSslStream.Entities;

namespace LibSocketAndSslStream.Interface;

public interface IAuthSslClient
{
    Task AuthenticateAsync(ObjSocketSslStream objSocketSslStream,
        CancellationToken cts = default);

    void Reconnect(Guid clientId);
}