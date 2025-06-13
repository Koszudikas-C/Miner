using System.Security.Cryptography.X509Certificates;

namespace DataFictitiousRemote.LibClass.LibCertificate;

public static class CertificateTest
{
    public static X509Certificate2 LoadCertificate()
    {
        var fileName = Environment.GetEnvironmentVariable("CERTIFICATE_PATH");
        var password = Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD");
        
        return new X509Certificate2(fileName!, password);
    }
}