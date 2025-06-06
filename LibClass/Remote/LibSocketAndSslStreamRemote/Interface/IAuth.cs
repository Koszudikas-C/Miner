using System.Net.Security;
using LibEntitiesRemote.Interface;

namespace LibSocketAndSslStreamRemote.Interface;

public interface IAuth
{
    Task<SslStream> AuthenticateAsync(ISocketWrapper socketWrapper,
        CancellationToken cts = default);
}
