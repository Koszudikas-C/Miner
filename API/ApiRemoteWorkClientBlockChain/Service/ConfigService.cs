using ApiRemoteWorkClientBlockChain.Interface;
using ApiRemoteWorkClientBlockChain.Utils;
using LibCryptographyRemote.Entities;
using LibCryptographyRemote.Interface;
using LibDtoRemote.Dto;
using LibEntitiesRemote.Entities;
using LibEntitiesRemote.Entities.Enum;
using LibManagerFileRemote.Entities.Enum;
using LibManagerFileRemote.Interface;
using LibMapperObjRemote.Interface;
using LibSendRemote.Interface;

namespace ApiRemoteWorkClientBlockChain.Service;

public class ConfigService(
  ILogger<ConfigService> logger,
  ISend<ConfigSaveFileDto> sendConfigSaveFileDto,
  ICryptographFile cryptographFile,
  ISearchFile searchFile,
  IMapperObj mapperObj)
  : IConfigService
{
  private readonly IMapperObj _mapperObj = mapperObj;

  public async Task<ConfigCryptograph> CreateEncryptedConfigAsync(ClientInfo clientInfo)
  {
    var cryptograph = PosAuth.PreparationFileCryptedConfigVariablePosAuth(clientInfo);
    var dataJson = await searchFile.SearchFileAsync(TypeFile.ConfigVariable);
    cryptograph.SetData(dataJson);
    var encryptedData = cryptographFile.SaveFile(cryptograph);
    cryptograph.FilePath = clientInfo.ClientMine!.PathLocal! + "Resources\\koewa";
    cryptograph.SetDataClear();
    cryptograph.SetDataBytes(encryptedData);
    return cryptograph;
  }

  public async Task SendConfigSaveFileAsync(ConfigCryptograph configCryptographDto, ClientInfo clientInfo)
  {
    try
    {
      var configSaveFileDto = PosAuth.ConfigSaveFileDtoPosAuth(clientInfo, configCryptographDto);
      await sendConfigSaveFileDto.SendAsync(configSaveFileDto, clientInfo, TypeSocketSsl.SslStream);
    }
    catch (Exception e)
    {
      logger.LogError($"Error sending configuration file: {e.Message}");
    }
  }
}
