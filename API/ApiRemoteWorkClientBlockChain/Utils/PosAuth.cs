using System.Security.Cryptography;
using LibCryptography.Entities;
using LibDto.Dto;
using LibDto.Dto.Enum;
using LibManagerFile.Entities;
using LibManagerFile.Entities.Enum;
using LibMapperObj.Interface;
using LibMapperObj.Service;
using LibRemoteAndClient.Entities.Remote.Client;

namespace ApiRemoteWorkClientBlockChain.Utils;

public static class PosAuth
{
    private static readonly string PathFile = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "koewa.json");

    private static readonly string PathFileDest =
        Path.Combine(Directory.GetCurrentDirectory(), "ResourcesDest", "koewa.json");

    
    public static ConfigSaveFileDto ConfigSaveFileDtoPosAuth(ClientInfo clientInfo,
        ConfigCryptograph configCryptograph)
    {
        var configSaveFileDto = new ConfigSaveFileDto()
        {
            FileName = $"koewa",
            DataBytes = configCryptograph.GetDataBytes(),
            PathFile = clientInfo.ClientMine!.PathLocal
        };

        configSaveFileDto.ExtensionFile = TypeExtensionFileDto.Json;

        return configSaveFileDto;
    }

    public static ConfigCryptograph PreparationFileCryptedConfigVariablePosAuth(ClientInfo clientInfo)
    {
        var configCrConfigVariable = new ConfigCryptograph(PathFileDest)
        {
            Key = RandomNumberGenerator.GetHexString(32),
            HmacKey = RandomNumberGenerator.GetHexString(32)
        };
        
        if (File.Exists(PathFileDest))
            File.Delete(PathFileDest);

        File.Copy(PathFile, PathFileDest);
        
        return configCrConfigVariable;
    }
}