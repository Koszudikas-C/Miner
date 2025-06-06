using System.Net.Security;

namespace LibEntitiesRemote.Interface;

public interface ISslStreamWrapper
{
    bool IsAuthenticated { get; }

    SslStream? InnerSslStream { get; }
    
}
