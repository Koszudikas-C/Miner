using System.Net.Security;

namespace LibSocketAndSslStreamRemote.Interface;

public interface ISslServerAuthOptions
{
    SslServerAuthenticationOptions GetConfigSslServerAuthenticationOptions();
}