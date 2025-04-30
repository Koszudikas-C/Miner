using System.Security.Cryptography.X509Certificates;

namespace LibCertificate.Interface;

public interface ICertificate
{
    X509Certificate2 LoadCertificate();
}