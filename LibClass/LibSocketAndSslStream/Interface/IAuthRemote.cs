using System.Net.Security;
using LibSocketAndSslStream.Interface;
using LibSocketAndSslStream.Entities.Enum;

namespace LibSocketAndSslStream.Interface;

public interface IAuthRemote
{
    Task<SslStream> AuthenticateAsync(ISocketWrapper socketWrapper,
        CancellationToken cts = default);
}
