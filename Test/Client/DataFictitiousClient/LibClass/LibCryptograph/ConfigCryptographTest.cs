using LibCryptographyClient.Entities;
using LibDtoClient.Dto;
using LibManagerFileClient.Entities.Enum;
using LibManagerFileClient.Interface;
using LibMapperObjClient.Interface;

namespace DataFictitiousClient.LibClass.LibCryptograph;

public class ConfigCryptographTest(ISearchFile searchFile, IMapperObj mapperObj)
{
    public ConfigCryptograph GetConfigCryptographConfigVariable()
    {
        var configGraphPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "koewa.json");
        var configCryptedDto = new ConfigCryptographDto();

        var data = searchFile.SearchFile(TypeFile.ConfigVariable);

        switch (data)
        {
            case ConfigVariableDto config:
                configCryptedDto.Data = config;
                break;
            case byte[] bytes:
                configCryptedDto.DataBytes = bytes;
                break;
            default:
                throw new FileNotFoundException(nameof(data));
        }

        return MapperObj(configCryptedDto, configGraphPath);
    }

    private ConfigCryptograph MapperObj(
        ConfigCryptographDto configCryptographDto, string filePath)
    {
        var configCryptograph = new ConfigCryptograph(filePath);

        var result = mapperObj.MapToObj(configCryptographDto, configCryptograph);
        
        if (configCryptographDto.Data is not null)
            result.SetData(configCryptographDto.Data);
        
        return result;
    }

    public ConfigCryptographDto GetConfigCryptographConfigVariableDto()
    {
        var configGraph = new ConfigCryptographDto();

        var data = searchFile.SearchFile(TypeFile.ConfigVariable);

        switch (data)
        {
            case ConfigVariableDto config:
                configGraph.Data = config;
                break;
            case byte[] bytes:
                configGraph.DataBytes = bytes;
                break;
            default:
                throw new FileNotFoundException(nameof(data));
        }

        return configGraph;
    }
}
