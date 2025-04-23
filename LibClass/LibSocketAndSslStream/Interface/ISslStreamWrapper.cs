using System.Net.Security;

namespace LibSocketAndSslStream.Interface;

public interface ISslStreamWrapper
{
    bool IsAuthenticated { get; }

    SslStream? InnerSslStream { get; }
    
}