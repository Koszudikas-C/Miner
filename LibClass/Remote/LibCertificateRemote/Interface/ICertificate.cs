using System.Security.Cryptography.X509Certificates;

namespace LibCertificateRemote.Interface;

public interface ICertificate
{
    X509Certificate2 LoadCertificate();
}
