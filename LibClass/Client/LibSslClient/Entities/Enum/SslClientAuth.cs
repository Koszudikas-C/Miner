using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using LibSocketAndSslStreamClient.Entities;
using LibSocketAndSslStreamClient.Interface;

namespace LibSslClient.Entities.Enum;

public class SslClientAuth(IConfigVariable configVariable)
{

    public SslClientAuthenticationOptions GetConfigSslClientAuthenticationOptions()
    {
        var config = configVariable.GetConfigVariable();
        var data = (ConfigVariable)config.GetData();
        return new SslClientAuthenticationOptions
        {
            TargetHost = data.RemoteSslBlock,
            ClientCertificates = null,
            CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
            EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
        };
    }
}
