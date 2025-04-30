using System.Net.Security;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Entities.Enum;

namespace LibSocketAndSslStream.Interface;

public interface IAuthSsl
{
    Task AuthenticateAsync(ObjSocketSslStream objSocketSslStream,
        CancellationToken cts = default);

    void Reconnect(Guid clientId);
}