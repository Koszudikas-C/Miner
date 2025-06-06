using System.Net.Security;
using LibEntitiesClient.Interface;

namespace LibSocketAndSslStreamClient.Interface;

public interface IAuth
{
    Task<SslStream> AuthenticateAsync(ISocketWrapper socketWrapper,
        CancellationToken cts = default);
}
