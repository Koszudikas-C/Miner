using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using LibSearchFile.Enum;

namespace LibSearchFile;

public static class SearchManager
{
    public static object? SearchFile(TypeFile type)
    {
        return type switch
        {
            TypeFile.Certificate => SearchCertificate(),
            _ => null,
        };
    }

    private static X509Certificate2 SearchCertificate()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? SearchCertificateWin()
            : SearchCertificateLinux();
    }

    private static X509Certificate2 SearchCertificateWin()
    {
        var locations = new StoreLocation[] { StoreLocation.CurrentUser,
            StoreLocation.LocalMachine };

        var stores = new StoreName[] { StoreName.My, StoreName.Root };

        foreach (var location in locations)
        {
            foreach (var store in stores)
            {
                using var x509Store = new X509Store(store, location);

                x509Store.Open(OpenFlags.ReadOnly);

                var certificate = x509Store.Certificates
                    .FirstOrDefault(x => x.Thumbprint.Replace(" ", "").Equals
                        ("b4bfd87fdc24723023984c74a6a2c744f5d5bb83", StringComparison.OrdinalIgnoreCase));

                if (certificate != null) return certificate;
            }
        }

        throw new Exception("Certificate file not found." +
                            "Checks if the certificate is installed.");
    }

    private static X509Certificate2 SearchCertificateLinux()
    {
        // var certPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "File", "Certificate", "Certificate.pfx");
        // const string certPassword = "88199299";
        
        var certPath = Environment.GetEnvironmentVariable("CERTIFICATE_PATH");
        var certPassword = Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD");

        Console.WriteLine($"CERTIFICATE_PATH: {Environment.GetEnvironmentVariable("CERTIFICATE_PATH")}");
        Console.WriteLine($"CERTIFICATE_PASSWORD: {Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD")}");

        if (certPath is null || certPassword is null) throw new ArgumentNullException(nameof(certPath),
            "CERTIFICATE_PATH, CERTIFICATE_PASSWORD is required.");
        
        if (File.Exists(certPath))
        {
            return new X509Certificate2(certPath, certPassword);
        }

        throw new FileNotFoundException("Certificate file not found." +
                                        "Checks if the certificate is on the File/Certificate/Certificate.pfx.");
    }
}
