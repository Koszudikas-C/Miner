using System.Runtime.Serialization;
using System.Text.Json;
using LibCryptographyRemote.Entities;
using LibCryptographyRemote.Interface;
using LibDtoRemote.Dto;
using LibManagerFileRemote.Entities.Enum;
using LibManagerFileRemote.Interface;
using LibMapperObjRemote.Interface;
using LibSocketAndSslStreamRemote.Entities;
using LibSocketAndSslStreamRemote.Interface;

namespace LibSocketRemote.Service;

public class ConfigVariableService(
  ICryptographFile cryptograph,
  ISearchFile searchFile,
  IMapperObj mapperObj) : IConfigVariable
{
  private readonly string _path = Path.Combine(
    AppDomain.CurrentDomain.BaseDirectory, "Resources", "koewa.json");

  public ConfigCryptograph GetConfigVariable()
  {
    var configCryptographDto = new ConfigCryptographDto();

    var data = searchFile.SearchFile(TypeFile.ConfigVariable);

    var configCryptograph = mapperObj.MapToObj(configCryptographDto, new ConfigCryptograph(_path));

    switch (data)
    {
      case ConfigVariableDto config:
        configCryptographDto.Data = config;
        configCryptograph.SetData(config);
        cryptograph.SaveFile(configCryptograph);
        break;
      case byte[] bytes:
        configCryptographDto.DataBytes = bytes;
        configCryptograph.SetDataBytes(bytes);
        break;
      default:
        throw new FileNotFoundException(nameof(data), "Data not found.");
    }

    return ProcessDecryptedData(configCryptograph);
  }

  private ConfigCryptograph ProcessDecryptedData(ConfigCryptograph data)
  {
    var result = cryptograph.LoadFile(data);

    var obj = JsonSerializer.Deserialize<ConfigVariableDto>(result);

    if (obj is null) throw new SerializationException(nameof(obj));

    var configVariable = mapperObj.MapToObj(obj, new ConfigVariable());

    data.SetData(configVariable);

    return data;
  }
}
