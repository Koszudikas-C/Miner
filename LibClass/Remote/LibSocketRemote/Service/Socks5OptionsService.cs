using System.Text.Json;
using LibCryptographyRemote.Entities;
using LibCryptographyRemote.Interface;
using LibManagerFileRemote.Entities.Enum;
using LibManagerFileRemote.Interface;
using LibSocketAndSslStreamRemote.Entities;
using LibSocketAndSslStreamRemote.Interface;
using LibSocks5Remote.Entities;

namespace LibSocketRemote.Service;

public class Socks5OptionsService(ICryptographFile cryptograph,
  ISearchFile searchFile) : ISocks5Options
{
    private readonly string _pathFile = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "Resources", "koewa.json");
    
    public Socks5Options GetConfigSocks5Options()
    {
        var configCryptograph = new ConfigCryptograph(_pathFile);

        var data = searchFile.SearchFile(TypeFile.ConfigVariable);
        
        switch (data)
        {
            case ConfigVariable configVariable:
                configCryptograph.SetData(configVariable);
                break;
            case byte[] bytes:
                configCryptograph.SetDataBytes(bytes);
                break;
            default:
                throw new FileNotFoundException(nameof(data),"Data not found."); 
        }
        
       return ProcessDecryptedData(configCryptograph);
    }

    private Socks5Options ProcessDecryptedData(ConfigCryptograph configCryptograph)
    {
        var result = cryptograph.LoadFile(configCryptograph);

        var obj = JsonSerializer.Deserialize<ConfigVariable>(result);
        
        if(obj is null)
            throw new NullReferenceException(nameof(obj));

        var configSocks5 = new Socks5Options(
            obj.ProxyHost!,
            obj.ProxyPort,
            obj.RemoteSocks5WorkService!,
            obj.RemoteSocks5DefaultPort,
            obj.RemoteUsernameTor!,
            obj.RemotePasswordTor!
            );
        
        return configSocks5;
    }
}
