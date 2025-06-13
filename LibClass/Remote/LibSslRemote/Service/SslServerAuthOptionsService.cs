using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using LibCertificateRemote.Interface;
using LibSocketAndSslStreamRemote.Interface;

namespace LibSslRemote.Service;

public class SslServerAuthOptionsService(ICertificate certificate) : ISslServerAuthOptions
{
    public SslServerAuthenticationOptions GetConfigSslServerAuthenticationOptions()
    {
        return new SslServerAuthenticationOptions
        {
            ClientCertificateRequired = false,
            CertificateRevocationCheckMode = X509RevocationMode.Online,
            EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
            EncryptionPolicy = EncryptionPolicy.RequireEncryption,
            ServerCertificate = certificate.LoadCertificate(),
        };
    }
}