using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using LibDtoRemote.Dto;
using LibManagerFileRemote.Entities.Enum;
using LibManagerFileRemote.Interface;

namespace LibSearchFileRemote.Service;

public class SearchFileService : ISearchFile
{
    private static readonly string PathFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
    
    public async Task<object> SearchFileAsync(TypeFile typeFile, CancellationToken cts = default)
    {
        return typeFile switch
        {
            TypeFile.Certificate => SearchCertificate(),
            TypeFile.ConfigVariable => await SearchConfigVariableAsync(cts),
            _ => throw new InvalidOperationException("Unknown file type"),
        };
    }
    
    private static async Task<object> SearchConfigVariableAsync(CancellationToken cts = default)
    {
        var configVariablePath = Path.Combine(PathFile, "koewa.json");
        
        try
        {
            if (!File.Exists(configVariablePath))
                throw new FileNotFoundException("Config variable file async not found."); 
            
            await using var stream = File.OpenRead(configVariablePath);
            
            var config = await JsonSerializer.DeserializeAsync<ConfigVariableDto>(stream, cancellationToken: cts);
            
            return config!;  
        }
        catch (JsonException)
        {
            return await File.ReadAllBytesAsync(configVariablePath, cts);
        }
    }
    
    public object SearchFile(TypeFile type)
    {
        return type switch
        {
            TypeFile.Certificate => SearchCertificate(),
            TypeFile.ConfigVariable => SearchConfigVariable(),
            _ => throw new FileNotFoundException()
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
        var locations = new []
        {
            StoreLocation.CurrentUser,
            StoreLocation.LocalMachine
        };

        var stores = new [] { StoreName.My, StoreName.Root };

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

        if (certPath is null || certPassword is null)
            throw new ArgumentNullException(nameof(certPath),
                "CERTIFICATE_PATH, CERTIFICATE_PASSWORD is required.");

        if (File.Exists(certPath))
        {
            return new X509Certificate2(certPath, certPassword);
        }

        throw new FileNotFoundException("Certificate file not found." +
                                        "Checks if the certificate is on the path:", certPath);
    }

    private static object SearchConfigVariable()
    {
        var configVariablePath = Path.Combine(PathFile, "koewa.json");
        
        try
        {
            if (!File.Exists(configVariablePath))
                throw new FileNotFoundException("Config variable file not found."); 
            
            var json = File.ReadAllText(configVariablePath);
            
            var config = JsonSerializer.Deserialize<ConfigVariableDto>(json);

            return config!;  
        }
        catch (JsonException)
        {
            return File.ReadAllBytes(configVariablePath);
        }
    }
}
