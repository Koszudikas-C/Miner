using System.Security.Cryptography;
using System.Text.Json;
using DataFictitious.Connection;
using LibCryptography.Entities;
using LibCryptography.Service;
using LibRemoteAndClient.Entities.Client;
using Xunit;

namespace TestWorkClientBlockChain.Service;

public class TestCryptographFileService
{
    private readonly CryptographFileService _mockCryptograph = new();
    private static string ApBaseDirectory => Path.Combine(
        Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "Resources", "koewa.json");
    
    private static readonly string DirectoryApp =  Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "Resources", "koewa.json");
    
    private static void Restore_test_file<T>()
    {
        var configCryptograph = new ConfigCryptograph<T>(DirectoryApp);
        
        if (File.Exists(configCryptograph.FilePath))
            File.Delete(configCryptograph.FilePath);
        
        File.Copy(ApBaseDirectory, configCryptograph.FilePath);
    }
    
    [Fact]
    public void Save_file_not_found_exception()
    {
        Restore_test_file<ConfigVariable>();
        var config = ConfigCryptographTest.GetConfigCryptograph();
        
        if (File.Exists(DirectoryApp)) File.Delete(DirectoryApp);
        
        Assert.Throws<FileNotFoundException>(() => _mockCryptograph.SaveFile(config));
    }
    
    [Fact]
    public void Save_file_config_variable_host()
    {
        Restore_test_file<ConfigVariable>();
        
        var config = ConfigCryptographTest.GetConfigCryptograph();
        
        var result = _mockCryptograph.SaveFile(config);
        Assert.NotNull(result);
        Assert.Equal(config.HeaderSignature, result.Take(config.HeaderSignature.Length).ToArray());
    }
    
    [Fact]
    public void Save_file_encrypted_exception()
    {
        Restore_test_file<ConfigVariable>();
        var config = ConfigCryptographTest.GetConfigCryptograph();
        
       _mockCryptograph.SaveFile(config);
        Assert.Throws<CryptographicException>(() => _mockCryptograph.SaveFile(config));
    }
    
    [Fact]
    public void Save_file_modify_header_signature_exception()
    {
        Restore_test_file<ConfigVariable>();
        var config = ConfigCryptographTest.GetConfigCryptograph();
        
        config.HeaderSignature = [0x4, 0x2, 0x3, 0x4];
        
        var result = _mockCryptograph.SaveFile(config);
        Assert.NotNull(result);
        Assert.Equal(config.HeaderSignature, result.Take(config.HeaderSignature.Length).ToArray());
    }
    
    [Fact]
    public void Load_file_config_Variable_host()
    {
        var config = ConfigCryptographTest.GetConfigCryptograph();
        
        const string host = "monerokoszudikas.duckdns.org";
        const int port = 5051;
        
        var result = _mockCryptograph.LoadFile(config);
        
        var resultJson = JsonSerializer.Deserialize<ConfigVariable>(result);

        Assert.NotNull(result);
        Assert.IsType<ConfigVariable>(resultJson!);
        Assert.Equal(host, resultJson.RemoteSslHost);
        Assert.Equal(port, resultJson.RemoteSslPort);
    }
    
    
    [Fact]
    public void Load_file_modify_data_exception()
    {
        var config = ConfigCryptographTest.GetConfigCryptograph();
        var data = config.GetDataBytes();
        var dataRaw = data.Skip(config.HeaderSignature.Length + 16)
            .Concat(new byte[] { 0x5, 0x6, 0x7, 0x8 })
            .ToArray();
        
        config.SetDataBytes(dataRaw);
        Assert.Throws<CryptographicException>(() => _mockCryptograph.LoadFile(config));
    }
    
    [Fact]
    public void Load_file_modify_header_variable_host_exception()
    {
        var config = ConfigCryptographTest.GetConfigCryptograph();
        
        config.HeaderSignature = [0x4, 0x2, 0x3, 0x4];
        
        Assert.Throws<CryptographicException>(() => _mockCryptograph.LoadFile(config));
    }
    
    [Fact]
    public void Load_file_modify_key_variable_host_exception()
    {
        var config = ConfigCryptographTest.GetConfigCryptograph();

        config.Key = "5694a6464s4d64a64564646544754746";
        
        Assert.Throws<CryptographicException>(() => _mockCryptograph.LoadFile(config));
    }
    
    [Fact]
    public void Load_file_modify_hmac_key_variable_host_exception()
    {
        var config = ConfigCryptographTest.GetConfigCryptograph();

        config.HmacKey = "5694a6464s4d64a64564646544754746";
        
        Assert.Throws<CryptographicException>(() => _mockCryptograph.LoadFile(config));
    }
}