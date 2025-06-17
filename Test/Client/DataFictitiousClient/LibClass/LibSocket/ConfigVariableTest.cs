using System.Runtime.Serialization;
using System.Text.Json;
using DataFictitiousClient.Entities;
using LibCryptographyClient.Entities;
using LibDtoClient.Dto;
using LibManagerFileClient.Entities.Enum;
using LibSocketAndSslStreamClient.Entities;

namespace DataFictitiousClient.LibClass.LibSocketClient;

public static class ConfigCryptographTest
{
    public static ConfigCryptograph GetConfigCryptograph()
    {
        var configCryptographDto = new ConfigCryptograph("Test");
        var configVariable = ConfigVariableTest.GetConfigVariable();
        
        configCryptographDto.SetData(configVariable);

        return configCryptographDto;
    }
}