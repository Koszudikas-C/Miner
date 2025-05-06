using LibCryptography.Entities;
using LibRemoteAndClient.Entities.Remote.Client;

namespace ApiRemoteWorkClientBlockChain.Interface
{
    public interface IConfigService
    {
        Task<ConfigCryptograph> CreateEncryptedConfigAsync(ClientInfo clientInfo);
        Task SendConfigSaveFileAsync(ConfigCryptograph configCryptographDto, ClientInfo clientInfo);
    }
}
