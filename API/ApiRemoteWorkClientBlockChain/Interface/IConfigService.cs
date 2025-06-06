using LibCryptographyRemote.Entities;
using LibEntitiesRemote.Entities;

namespace ApiRemoteWorkClientBlockChain.Interface
{
    public interface IConfigService
    {
        Task<ConfigCryptograph> CreateEncryptedConfigAsync(ClientInfo clientInfo);
        Task SendConfigSaveFileAsync(ConfigCryptograph configCryptographDto, ClientInfo clientInfo);
    }
}
