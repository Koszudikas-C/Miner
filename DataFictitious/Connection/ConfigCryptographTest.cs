using System.Runtime.InteropServices;
using LibCryptography.Entities;
using LibRemoteAndClient.Entities.Client;
using LibSearchFile;
using LibSearchFile.Enum;

namespace DataFictitious.Connection;

public static class ConfigCryptographTest
{
    public static ConfigCryptograph<ConfigVariable> GetConfigCryptograph()
    {
        var configGraphPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "koewa.json");
        var configGraph = new ConfigCryptograph<ConfigVariable>(configGraphPath);

        var data = SearchManager.SearchFile(TypeFile.ConfigVariable);
        
        switch (data)
        {
            case ConfigVariable config:
                configGraph.SetData(config);
                break;
            case byte[] bytes:
                configGraph.SetDataBytes(bytes);
                break;
            default:
                throw new FileNotFoundException(nameof(data));
        }
        
        return configGraph;
    }
}