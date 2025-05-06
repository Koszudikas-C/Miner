using ApiRemoteWorkClientBlockChain.Entities.Interface;
using ApiRemoteWorkClientBlockChain.Interface;
using ApiRemoteWorkClientBlockChain.Utils;
using LibCryptography.Entities;
using LibCryptography.Interface;
using LibDto.Dto;
using LibManagerFile.Entities.Enum;
using LibManagerFile.Interface;
using LibMapperObj.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSend.Interface;

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
