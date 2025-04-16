using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace LibCertificate;

public static class ValidateCertificate
{
    public static bool CertificateValidationCallBack(object sender,
        X509Certificate certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
    {
        return sslPolicyErrors == SslPolicyErrors.None && ValidateCertificateNew(chain!, certificate);
    }

    private static bool ValidateCertificateNew(X509Chain? chain, X509Certificate? certificate)
    {
        if (chain == null) return true;
        
        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;

        if (chain.Build((X509Certificate2)certificate!)) return true;
        foreach (var status in chain.ChainStatus)
        {
        }
        
        return false;
    }
}