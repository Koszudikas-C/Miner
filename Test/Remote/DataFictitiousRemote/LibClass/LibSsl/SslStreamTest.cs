using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace DataFictitiousRemote.LibClass.LibSsl;

public static class SslStreamTest
{
    public static SslServerAuthenticationOptions GetConfigSslServerAuthenticationOptions(X509Certificate2 certificate)
    {
        return new SslServerAuthenticationOptions
        {
            ClientCertificateRequired = false,
            CertificateRevocationCheckMode = X509RevocationMode.Online,
            EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
            EncryptionPolicy = EncryptionPolicy.RequireEncryption,
            ServerCertificate = certificate,
        };
    }
}