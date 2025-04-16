using System.Security;
using System.Security.Cryptography.X509Certificates;
using LibSearchFile;
using LibSearchFile.Enum;

namespace LibCertificate;

public static class Certificate
{
     public static X509Certificate2 LoadCertificate()
    {
        var certificate = 
            SearchManager.SearchFile(TypeFile.Certificate) as X509Certificate2;
        
        if(!IsValid(certificate!))
            throw new SecurityException("Certificate is not valid.");
        
        return certificate!;
    }

    private static bool IsValid(X509Certificate2? certificate)
    {
        if (certificate == null) return false;

        var isExpired = DateTime.UtcNow < certificate.NotBefore || DateTime.UtcNow > certificate.NotAfter;
        var hasPrivateKey = certificate.HasPrivateKey;
        var isSelfSigned = certificate.Subject == certificate.Issuer;

        return !isExpired && hasPrivateKey && !isSelfSigned;
    }
}
