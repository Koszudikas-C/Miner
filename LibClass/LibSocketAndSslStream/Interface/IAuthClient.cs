using System.Net.Security;
using LibSocketAndSslStream.Entities.Enum;

namespace LibSocketAndSslStream.Interface;

public interface IAuthClient
{
    Task<SslStream> AuthenticateAsync(ISocketWrapper socketWrapper,
        CancellationToken cts = default);
}
