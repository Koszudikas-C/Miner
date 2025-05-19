using System.Net;
using ApiRemoteWorkClientBlockChain.Entities.Interface;
using ApiRemoteWorkClientBlockChain.Interface;
using LibClassManagerOptions.Entities;
using LibClassManagerOptions.Entities.Enum;
using LibClassManagerOptions.Interface;
using LibClassProcessOperations.Entities;
using LibCommunicationStatus.Entities;
using LibDto.Dto;
using LibRemoteAndClient.Enum;
using LibSend.Interface;

namespace ApiRemoteWorkClientBlockChain.Service;

public class PosAuthService(
    ILogger<PosAuthService> logger,
    IClientConnected clientConnected,
    ISend<ConfigCryptographDto> sendConfigCryptographDto,
    IManagerOptions<ParamsSocks5> managerOptions)
    : IPosAuth
{
    private readonly IManagerOptions<ParamsSocks5> _managerOptions = managerOptions;
    
    public async Task SendDataAsync(ConfigCryptographDto configCryptographDto,
        Guid clientId, CancellationToken cts = default)
    {
        try
        {
            var clientInfo = clientConnected.GetClientInfo(clientId);
            
            await sendConfigCryptographDto.SendAsync(configCryptographDto, clientInfo, TypeSocketSsl.SslStream, cts);
            
            logger.LogInformation("Successful encrypted configuration was sent.");

            var paramsSocks5 = new ParamsSocks5();
            paramsSocks5.ParamsGetProcessInfo.Port = 9050;
            paramsSocks5.ParamsGetProcessInfo.NameProcess = "tor";
            var paramsManagerOptionsSocks5 = new ParamsManagerOptions<ParamsSocks5>(TypeManagerOptions.AuthSocks5, paramsSocks5);
            
            await _managerOptions.InitializeOptionsAsync(paramsManagerOptionsSocks5, cts);
        }
        catch (Exception e)
        {
            logger.LogError($"Error sending data: {e.Message}");
        }
    }
}
