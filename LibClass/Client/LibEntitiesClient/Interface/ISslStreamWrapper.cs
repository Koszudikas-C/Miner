using System.Net.Security;

namespace LibEntitiesClient.Interface;

public interface ISslStreamWrapper
{
    bool IsAuthenticated { get; }

    SslStream? InnerSslStream { get; }
    
}
