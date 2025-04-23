using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using LibRemoteAndClient.Entities.Client;
using LibSearchFile.Enum;

namespace LibSearchFile;

public static class SearchManager
{
    private readonly static string PathFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");

    public static object? SearchFile(TypeFile type)
    {
        return type switch
        {
            TypeFile.Certificate => SearchCertificate(),
            TypeFile.ConfigVariable => SearchConfigVariable(),
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
        var locations = new StoreLocation[]
        {
            StoreLocation.CurrentUser,
            StoreLocation.LocalMachine
        };

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
        var certPath = Environment.GetEnvironmentVariable("CERTIFICATE_PATH");
        var certPassword = Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD");

        Console.WriteLine($"CERTIFICATE_PATH: {Environment.GetEnvironmentVariable("CERTIFICATE_PATH")}");
        Console.WriteLine($"CERTIFICATE_PASSWORD: {Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD")}");

        if (certPath is null || certPassword is null)
            throw new ArgumentNullException(nameof(certPath),
                "CERTIFICATE_PATH, CERTIFICATE_PASSWORD is required.");

        if (File.Exists(certPath))
        {
            return new X509Certificate2(certPath, certPassword);
        }

        throw new FileNotFoundException("Certificate file not found." +
                                        "Checks if the certificate is on the File/Certificate/Certificate.pfx.");
    }

    private static object? SearchConfigVariable()
    {
        var configVariablePath = Path.Combine(PathFile, "koewa.json");
        
        try
        {
            if (!File.Exists(configVariablePath)) return null;
            
            var json = File.ReadAllText(configVariablePath);
            
            var config = JsonSerializer.Deserialize<ConfigVariable>(json);

            return config;  
        }
        catch (JsonException)
        {
            return File.ReadAllBytes(configVariablePath);
        }
    }
}