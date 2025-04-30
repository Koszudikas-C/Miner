using System.Text;
using LibManagerFile.Entities;
using LibManagerFile.Entities.Enum;

namespace DataFictitious.LibClass.LibSaveFile;

public static class ConfigSaveFileTest
{
    public static ConfigSaveFile GetConfigSaveFileTest()
    {
        var json = """
                   {
                       "Name": "Example",
                       "Enabled": true,
                       "MaxConnections": 10,
                       "Servers": [
                           "server1.example.com",
                           "server2.example.com"
                       ],
                       "Settings": {
                           "Timeout": 30,
                           "Retry": 5
                       }
                   }
                   """;

        var configSaveFile = new ConfigSaveFile()
        {
            FileName = "test",
            Data = json,
            DataBytes = Encoding.UTF8.GetBytes(json)
        };
        
        configSaveFile.SetExtension(TypeExtensionFile.Json);
        
        return configSaveFile;
    }
    
}